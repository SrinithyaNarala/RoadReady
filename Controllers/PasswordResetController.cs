using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RoadReady.Repositories;
using RoadReady.Models;
using AutoMapper;
using System;
using System.Threading.Tasks;
using RoadReady.Authentication;
using System.Net.Mail;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace RoadReady.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PasswordResetController : ControllerBase
    {
        private readonly IEmailRepository _emailRepository;
        private readonly RoadReadyContext _dbContext;
        private readonly IMapper _mapper;

        public PasswordResetController(IEmailRepository emailRepository, RoadReadyContext dbContext, IMapper mapper)
        {
            _emailRepository = emailRepository;
            _dbContext = dbContext;
            _mapper = mapper;
        }

        [AllowAnonymous]
        [HttpPost("send-reset-email")]
        public async Task<IActionResult> SendPasswordResetEmail(string email)
        {
            try
            {
                // Find user by email
                var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == email);
                if (user == null)
                {
                    return NotFound("User with the provided email does not exist.");
                }

                // Generate reset token and expiration date (UTC)
                var resetToken = Guid.NewGuid().ToString();
                var expirationDate = DateTime.UtcNow.AddHours(1);
                // Convert UTC expiration date to IST (India Standard Time)
                var indiaTimeZone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
                var localExpirationDate = TimeZoneInfo.ConvertTimeFromUtc(expirationDate, indiaTimeZone);

                // Format the expiration date in a readable format (e.g., "yyyy-MM-dd HH:mm:ss")
                var formattedExpirationDate = localExpirationDate.ToString("yyyy-MM-dd HH:mm:ss");
                // Create and save PasswordReset entity
                var passwordReset = new PasswordReset
                {
                    UserId = user.UserId,
                    ResetToken = resetToken,
                    ExpirationDate = expirationDate,
                    IsUsed = false
                };

                _dbContext.PasswordResets.Add(passwordReset);
                await _dbContext.SaveChangesAsync();

                // Map to DTO and send email
                var resetDto = _mapper.Map<PasswordResetDTO>(passwordReset);
                var subject = "Password Reset Request";
                // Construct the email body with the correctly formatted expiration time
                var body = $"Click the link below to reset your password:\n" +
                           $"https://roadready.com/reset-password?token={resetToken}\n" +
                           $"Token expires at {formattedExpirationDate}.";

                try
                {
                    await _emailRepository.SendEmailAsync(email, subject, body);
                    return Ok("Reset email sent successfully.");
                }
                catch (SmtpException smtpEx)
                {
                    // Log the exception details
                    return StatusCode(500, $"SMTP error: {smtpEx.Message}\n{smtpEx.InnerException?.Message}");
                }
                catch (Exception ex)
                {
                    // Log general exception
                    return StatusCode(500, $"Error sending email: {ex.Message}\n{ex.InnerException?.Message}");
                }
            }
            catch (SmtpException smtpEx)
            {
                // Log the SMTP exception for debugging
                return StatusCode(500, $"SMTP error: {smtpEx.Message}");
            }
            catch (Exception ex)
            {
                // Log other general exceptions for debugging
                return StatusCode(500, $"Error sending email: {ex.Message}");
            }
        }


        [HttpPost("reset-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword(string resetToken, string newPassword)
        {
            try
            {
                // Decode the token from the URL if it's encoded
                var decodedToken = Uri.UnescapeDataString(resetToken);

                // Validate the reset token
                var passwordReset = await _dbContext.PasswordResets
                    .Include(pr => pr.User)
                    .FirstOrDefaultAsync(pr => pr.ResetToken == decodedToken);

                if (passwordReset == null)
                {
                    return BadRequest("Invalid reset token. The token does not exist.");
                }

                // Check if the token has already been used
                if (passwordReset.IsUsed)
                {
                    return BadRequest("This reset token has already been used.");
                }

                // Check if the token has expired
                if (passwordReset.ExpirationDate < DateTime.UtcNow)
                {
                    return BadRequest("The reset token has expired.");
                }

                // Ensure the user is found
                var user = passwordReset.User;
                if (user == null)
                {
                    return BadRequest("User associated with this token not found.");
                }

                // Hash the new password securely (using a strong hashing algorithm)
                var passwordHasher = new PasswordHasher<User>();
                user.Password = passwordHasher.HashPassword(user, newPassword);

                // Mark the reset token as used
                passwordReset.IsUsed = true;

                // Save the changes in the database
                await _dbContext.SaveChangesAsync();

                return Ok("Password has been reset successfully.");
            }
            catch (Exception ex)
            {
                // Return an error if something goes wrong
                return StatusCode(500, $"An error occurred while resetting the password: {ex.Message}");
            }
        }





        // Endpoint to get all password reset records (for admin or debugging purposes)
        [HttpGet("all-resets")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllPasswordResets()
        {
            var resets = await _dbContext.PasswordResets
                .Include(pr => pr.User)
                .ToListAsync();

            var resetDtos = _mapper.Map<List<PasswordResetDTO>>(resets);
            return Ok(resetDtos);
        }

        // Endpoint to get a single password reset record by token
        [HttpGet("reset/{resetToken}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetPasswordResetByToken(string resetToken)
        {
            var passwordReset = await _dbContext.PasswordResets
                .Include(pr => pr.User)
                .FirstOrDefaultAsync(pr => pr.ResetToken == resetToken);

            if (passwordReset == null)
            {
                return NotFound("Password reset record not found.");
            }

            var resetDto = _mapper.Map<PasswordResetDTO>(passwordReset);
            return Ok(resetDto);
        }
    }
}
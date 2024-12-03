using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using RoadReady.Authentication;
using RoadReady.Models;

namespace RoadReady.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;

        public AuthenticationController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var users = await _userManager.FindByNameAsync(model.UserName);
            if (users != null && await _userManager.CheckPasswordAsync(users, model.Password))
            {
                var userRoles = await _userManager.GetRolesAsync(users);
                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, users.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                };
                foreach (var userRole in userRoles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                }

                var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
                var token = new JwtSecurityToken(
                    issuer: _configuration["Jwt:Issuer"],
                    audience: _configuration["Jwt:Audience"],
                    expires: DateTime.Now.AddHours(5),
                    claims: authClaims,
                    signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );

                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token),
                    expiration = token.ValidTo
                });
            }
            return Unauthorized();
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            if (string.IsNullOrEmpty(model.Role))
                return BadRequest(new Response { Status = "Error", Message = "Role is required." });

            var userExists = await _userManager.FindByNameAsync(model.UserName);
            if (userExists != null)
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User already exists!" });

            ApplicationUser user = new ApplicationUser()
            {
                UserName = model.UserName,
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User creation failed: " + errors });
            }

            if (!await _roleManager.RoleExistsAsync(model.Role))
            {
                var roleResult = await _roleManager.CreateAsync(new IdentityRole(model.Role));
                if (!roleResult.Succeeded)
                {
                    var roleErrors = string.Join(", ", roleResult.Errors.Select(e => e.Description));
                    return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Role creation failed: " + roleErrors });
                }
            }

            await _userManager.AddToRoleAsync(user, model.Role);
            return Ok(new Response { Status = "Success", Message = $"{model.Role} user created successfully!" });
        }

    }
}
using RoadReady.Repositories;
using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

public class EmailRepository : IEmailRepository
{
    private readonly string _smtpHost = "smtp.gmail.com";
    private readonly int _smtpPort = 587;
    private readonly string _smtpUser = "vijayasrichakiri01@gmail.com";
    private readonly string _smtpPassword = "lbwd zoee phlk nbyi"; // Use the App Password
    private readonly bool _enableSsl = true;

    public async Task SendEmailAsync(string toEmail, string subject, string body)
    {
        try
        {
            using (var smtpClient = new SmtpClient(_smtpHost)
            {
                Port = _smtpPort,
                Credentials = new NetworkCredential(_smtpUser, _smtpPassword),
                EnableSsl = _enableSsl
            })
            {
                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_smtpUser),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };
                mailMessage.To.Add(toEmail);

                await smtpClient.SendMailAsync(mailMessage);
            }
        }
        catch (SmtpException smtpEx)
        {
            Console.WriteLine("SMTP Error: " + smtpEx.Message);
            Console.WriteLine("Inner Exception: " + smtpEx.InnerException);
            throw new Exception("Error sending email: " + smtpEx.Message, smtpEx);
        }
        catch (Exception ex)
        {
            Console.WriteLine("General Error: " + ex.Message);
            throw new Exception("Error sending email: " + ex.Message, ex);
        }
    }
}
namespace RoadReady.Repositories
{
    public interface IEmailRepository
    {
        Task SendEmailAsync(string toEmail, string subject, string body);
    }
}
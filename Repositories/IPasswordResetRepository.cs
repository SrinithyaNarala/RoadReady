using RoadReady.Models;

namespace RoadReady.Repositories
{
    public interface IPasswordResetRepository
    {
        Task<IEnumerable<PasswordReset>> GetAllResetsAsync();
        Task<PasswordReset?> GetResetByIdAsync(int resetId);
        Task<PasswordReset?> GetResetByTokenAsync(string token);
        Task AddResetAsync(PasswordReset passwordReset);
        Task UpdateResetAsync(PasswordReset passwordReset);
        Task DeleteResetAsync(int resetId);


    }
}
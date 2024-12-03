using RoadReady.Models;

namespace RoadReady.Repositories
{
    public interface IUserRepository
    {
        Task<User> GetUserByIdAsync(int id);
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task AddUserAsync(User users);
        Task UpdateUserAsync(User users);
        Task DeleteUserAsync(int id);

    }
}
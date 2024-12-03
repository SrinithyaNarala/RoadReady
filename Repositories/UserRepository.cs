using Microsoft.EntityFrameworkCore;
using RoadReady.Authentication;
using RoadReady.Models;
using RoadReady.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class UserRepository : IUserRepository
{
    private readonly RoadReadyContext _context;

    public UserRepository(RoadReadyContext context)
    {
        _context = context;
    }


    public async Task<User> GetUserByIdAsync(int id)
    {
        return await _context.Users.FindAsync(id);
    }

    public async Task<IEnumerable<User>> GetAllUsersAsync()
    {
        return await _context.Users.ToListAsync();
    }

    public async Task AddUserAsync(User users)
    {
        await _context.Users.AddAsync(users);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateUserAsync(User users)
    {
        _context.Users.Update(users);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteUserAsync(int id)
    {
        var users = await GetUserByIdAsync(id);
        if (users != null)
        {
            _context.Users.Remove(users);
            await _context.SaveChangesAsync();
        }
    }
}
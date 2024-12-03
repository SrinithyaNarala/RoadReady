using Microsoft.EntityFrameworkCore;
using RoadReady.Authentication;
using RoadReady.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RoadReady.Repositories
{
    public class PasswordResetRepository : IPasswordResetRepository
    {
        private readonly RoadReadyContext _context;

        public PasswordResetRepository(RoadReadyContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<PasswordReset>> GetAllResetsAsync()
        {
            return await _context.PasswordResets.ToListAsync();
        }

        public async Task<PasswordReset?> GetResetByIdAsync(int resetId)
        {
            return await _context.PasswordResets.FindAsync(resetId);
        }

        public async Task<PasswordReset?> GetResetByTokenAsync(string token)
        {
            return await _context.PasswordResets
                .FirstOrDefaultAsync(pr => pr.ResetToken == token);
        }

        public async Task AddResetAsync(PasswordReset passwordReset)
        {
            _context.PasswordResets.Add(passwordReset);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateResetAsync(PasswordReset passwordReset)
        {
            _context.PasswordResets.Update(passwordReset);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteResetAsync(int resetId)
        {
            var reset = await GetResetByIdAsync(resetId);
            if (reset != null)
            {
                _context.PasswordResets.Remove(reset);
                await _context.SaveChangesAsync();
            }
        }
    }
}
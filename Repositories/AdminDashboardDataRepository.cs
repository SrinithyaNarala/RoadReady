using RoadReady.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using RoadReady.Authentication;

namespace RoadReady.Repositories
{
    public class AdminDashboardDataRepository : IAdminDashboardDataRepository
    {
        private readonly RoadReadyContext _context;

        public AdminDashboardDataRepository(RoadReadyContext context)
        {
            _context = context;
        }

        public async Task AddDashboardDataAsync(AdminDashboardData dashboardData)
        {
            await _context.AdminDashboardData.AddAsync(dashboardData);
            await _context.SaveChangesAsync();
        }

        public IEnumerable<AdminDashboardData> GetAllAdminDashboardData()
        {
            return _context.AdminDashboardData.ToList();
        }

        public async Task<AdminDashboardData> GetDashboardDataByIdAsync(int id)
        {
            return await _context.AdminDashboardData.FindAsync(id);
        }

        public async Task UpdateDashboardDataAsync(AdminDashboardData dashboardData)
        {
            // Update the existing dashboard data record
            _context.AdminDashboardData.Update(dashboardData);
            await _context.SaveChangesAsync();
        }
    }
}
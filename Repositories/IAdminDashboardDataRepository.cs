using RoadReady.Models;

namespace RoadReady.Repositories
{
    public interface IAdminDashboardDataRepository
    {
        IEnumerable<AdminDashboardData> GetAllAdminDashboardData();

        Task AddDashboardDataAsync(AdminDashboardData dashboardData);
        Task<AdminDashboardData> GetDashboardDataByIdAsync(int id);

        Task UpdateDashboardDataAsync(AdminDashboardData dashboardData);
    }
}
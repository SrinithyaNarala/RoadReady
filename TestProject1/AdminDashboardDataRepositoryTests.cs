using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using RoadReady.Authentication;
using RoadReady.Models;
using RoadReady.Repositories;
using System.Linq;
using System.Threading.Tasks;

namespace TestProject1
{

    [TestFixture]
    public class AdminDashboardDataRepositoryTests
    {
        private RoadReadyContext _context;
        private AdminDashboardDataRepository _dashboardRepository;

        [SetUp]
        public void Setup()
        {
            // Create unique in-memory DbContextOptions for test isolation
            var options = new DbContextOptionsBuilder<RoadReadyContext>()
                .UseInMemoryDatabase(databaseName: $"TestDatabase_{System.Guid.NewGuid()}")
                .Options;

            // Initialize DbContext with in-memory options
            _context = new RoadReadyContext(options);

            // Seed data for testing
            _context.AdminDashboardData.AddRange(
                new AdminDashboardData { DashboardId = 1, TotalReservations = 100, TotalRevenue = 5000.50m },
                new AdminDashboardData { DashboardId = 2, TotalReservations = 200, TotalRevenue = 10000.75m }
            );
            _context.SaveChanges();

            // Initialize repository with context
            _dashboardRepository = new AdminDashboardDataRepository(_context);
        }

        [TearDown]
        public void Cleanup()
        {
            // Dispose DbContext to clean up resources
            _context.Dispose();
        }

        [Test]
        public async Task AddDashboardDataAsync_ShouldAddDashboardData()
        {
            // Arrange
            var newDashboardData = new AdminDashboardData
            {
                DashboardId = 3,
                TotalReservations = 300,
                TotalRevenue = 15000.25m,
                TotalUsers = 150,
                TotalCars = 50,
                TotalReviews = 75,
                CreatedAt = System.DateTime.Now
            };

            // Act
            await _dashboardRepository.AddDashboardDataAsync(newDashboardData);

            // Assert
            var result = await _dashboardRepository.GetDashboardDataByIdAsync(3);
            Assert.IsNotNull(result, "Expected added dashboard data to be found but was null.");
            Assert.AreEqual(300, result.TotalReservations, "Added dashboard data total reservations does not match.");
            Assert.AreEqual(15000.25m, result.TotalRevenue, "Added dashboard data total revenue does not match.");
        }

        [Test]
        public void GetAllAdminDashboardData_ShouldReturnAllDashboardData()
        {
            // Act
            var result = _dashboardRepository.GetAllAdminDashboardData();

            // Assert
            Assert.IsNotNull(result, "Expected non-null result but got null.");
            Assert.AreEqual(2, result.Count(), "Expected two dashboard data entries but found a different count.");
        }

        [Test]
        public async Task GetDashboardDataByIdAsync_ShouldReturnDashboardData_WhenDataExists()
        {
            // Act
            var result = await _dashboardRepository.GetDashboardDataByIdAsync(1);

            // Assert
            Assert.IsNotNull(result, "Expected dashboard data to be found but was null.");
            Assert.AreEqual(100, result.TotalReservations, "Dashboard data total reservations does not match.");
            Assert.AreEqual(5000.50m, result.TotalRevenue, "Dashboard data total revenue does not match.");
        }

        [Test]
        public async Task UpdateDashboardDataAsync_ShouldUpdateDashboardData()
        {
            // Arrange
            var existingData = await _dashboardRepository.GetDashboardDataByIdAsync(1);
            existingData.TotalReservations = 150;

            // Act
            await _dashboardRepository.UpdateDashboardDataAsync(existingData);

            // Assert
            var result = await _dashboardRepository.GetDashboardDataByIdAsync(1);
            Assert.AreEqual(150, result.TotalReservations, "Updated dashboard data total reservations does not match.");
        }
    }
}
using Microsoft.EntityFrameworkCore;
using RoadReady.Authentication;
using RoadReady.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestProject1
{
    internal class UserRepositoryTests
    {
        private RoadReadyContext _context;
        private UserRepository _userRepository;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<RoadReadyContext>()
                .UseInMemoryDatabase(databaseName: "UserTestDatabase")
                .Options;

            _context = new RoadReadyContext(options);
            _userRepository = new UserRepository(_context);

            // Ensure the database is cleared before adding new data
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();

            // Seed data for testing
            SeedDatabase();
        }

        private void SeedDatabase()
        {
            // Remove any existing users in the context to avoid key conflicts
            if (_context.Users.Any())
            {
                _context.Users.RemoveRange(_context.Users);
                _context.SaveChanges();
            }

            _context.Users.AddRange(
                new User { UserId = 1, FirstName = "John", LastName = "Doe", Email = "john@example.com", Role = "Customer", CreatedAt = DateTime.Now },
                new User { UserId = 2, FirstName = "Jane", LastName = "Smith", Email = "jane@example.com", Role = "Admin", CreatedAt = DateTime.Now }
            );
            _context.SaveChanges();
        }

        [Test]
        public async Task GetUserByIdAsync_ShouldReturnCorrectUser()
        {
            // Act
            var user = await _userRepository.GetUserByIdAsync(1);

            // Assert
            Assert.IsNotNull(user, "Expected a user but got null.");
            Assert.AreEqual(1, user.UserId, "UserId mismatch.");
            Assert.AreEqual("John", user.FirstName, "FirstName mismatch.");
        }

        [Test]
        public async Task GetAllUsersAsync_ShouldReturnAllUsers()
        {
            // Act
            var users = await _userRepository.GetAllUsersAsync();

            // Assert
            Assert.AreEqual(2, users.Count(), "Expected two users.");
        }

        [Test]
        public async Task AddUserAsync_ShouldAddNewUser()
        {
            // Arrange
            var newUser = new User
            {
                UserId = 3,
                FirstName = "Alice",
                LastName = "Brown",
                Email = "alice@example.com",
                Role = "Customer",
                CreatedAt = DateTime.Now
            };

            // Act
            await _userRepository.AddUserAsync(newUser);
            var addedUser = await _userRepository.GetUserByIdAsync(3);

            // Assert
            Assert.IsNotNull(addedUser, "Expected a user but got null.");
            Assert.AreEqual("Alice", addedUser.FirstName, "FirstName mismatch.");
        }

        [Test]
        public async Task UpdateUserAsync_ShouldUpdateExistingUser()
        {
            // Arrange
            var existingUser = await _userRepository.GetUserByIdAsync(1);
            existingUser.LastName = "Updated";

            // Act
            await _userRepository.UpdateUserAsync(existingUser);
            var updatedUser = await _userRepository.GetUserByIdAsync(1);

            // Assert
            Assert.AreEqual("Updated", updatedUser.LastName, "LastName mismatch.");
        }

        [Test]
        public async Task DeleteUserAsync_ShouldDeleteUser()
        {
            // Act
            await _userRepository.DeleteUserAsync(1);
            var deletedUser = await _userRepository.GetUserByIdAsync(1);

            // Assert
            Assert.IsNull(deletedUser, "Expected null but got a user.");
        }
    }
}

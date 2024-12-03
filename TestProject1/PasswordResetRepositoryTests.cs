using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using RoadReady.Authentication;
using RoadReady.Models;
using RoadReady.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RoadReady.Tests
{
    public class PasswordResetRepositoryTests
    {
        private RoadReadyContext _context;
        private PasswordResetRepository _passwordResetRepository;
        private List<PasswordReset> _passwordResets;

        [SetUp]
        public void Setup()
        {
            // Create a new in-memory database and DbContext
            var options = new DbContextOptionsBuilder<RoadReadyContext>()
                .UseInMemoryDatabase(databaseName: "RoadReadyTestDatabase") // Use a unique in-memory database name
                .Options;

            // Initialize the context with in-memory database
            _context = new RoadReadyContext(options);

            // Seed data for testing
            _passwordResets = new List<PasswordReset>
            {
                new PasswordReset { ResetId = 1, UserId = 1, ResetToken = "token123", ExpirationDate = DateTime.Now.AddHours(1), IsUsed = false },
                new PasswordReset { ResetId = 2, UserId = 2, ResetToken = "token456", ExpirationDate = DateTime.Now.AddHours(1), IsUsed = false }
            };

            // Add data to in-memory database
            _context.PasswordResets.AddRange(_passwordResets);
            _context.SaveChanges();

            // Initialize the repository with the in-memory context
            _passwordResetRepository = new PasswordResetRepository(_context);
        }

        [Test]
        public async Task GetAllResetsAsync_ShouldReturnAllPasswordResets()
        {
            // Act
            var resets = await _passwordResetRepository.GetAllResetsAsync();

            // Assert
            Assert.NotNull(resets);
            Assert.AreEqual(2, resets.Count());
        }

        [Test]
        public async Task GetResetByIdAsync_ShouldReturnCorrectReset()
        {
            // Act
            var reset = await _passwordResetRepository.GetResetByIdAsync(1);

            // Assert
            Assert.NotNull(reset);
            Assert.AreEqual(1, reset.ResetId);
            Assert.AreEqual("token123", reset.ResetToken);
        }

        [Test]
        public async Task GetResetByIdAsync_ShouldReturnNull_WhenResetNotFound()
        {
            // Act
            var reset = await _passwordResetRepository.GetResetByIdAsync(3);

            // Assert
            Assert.IsNull(reset);
        }

        [Test]
        public async Task GetResetByTokenAsync_ShouldReturnCorrectReset()
        {
            // Act
            var reset = await _passwordResetRepository.GetResetByTokenAsync("token123");

            // Assert
            Assert.NotNull(reset);
            Assert.AreEqual("token123", reset.ResetToken);
            Assert.AreEqual(1, reset.ResetId);
        }

        [Test]
        public async Task GetResetByTokenAsync_ShouldReturnNull_WhenTokenNotFound()
        {
            // Act
            var reset = await _passwordResetRepository.GetResetByTokenAsync("nonexistentToken");

            // Assert
            Assert.IsNull(reset);
        }

        [Test]
        public async Task AddResetAsync_ShouldAddNewPasswordReset()
        {
            // Arrange
            var newPasswordReset = new PasswordReset { ResetId = 3, UserId = 3, ResetToken = "token789", ExpirationDate = DateTime.Now.AddHours(1), IsUsed = false };

            // Act
            await _passwordResetRepository.AddResetAsync(newPasswordReset);

            // Assert
            var addedReset = await _context.PasswordResets.FindAsync(3);
            Assert.NotNull(addedReset);
            Assert.AreEqual("token789", addedReset.ResetToken);
        }

        [Test]
        public async Task UpdateResetAsync_ShouldUpdateExistingPasswordReset()
        {
            // Arrange
            var resetToUpdate = _passwordResets.First();
            resetToUpdate.IsUsed = true;

            // Act
            await _passwordResetRepository.UpdateResetAsync(resetToUpdate);

            // Assert
            var updatedReset = await _context.PasswordResets.FindAsync(resetToUpdate.ResetId);
            Assert.NotNull(updatedReset);
            Assert.IsTrue(updatedReset.IsUsed);
        }

        [Test]
        public async Task DeleteResetAsync_ShouldDeletePasswordReset()
        {
            // Act
            await _passwordResetRepository.DeleteResetAsync(1);

            // Assert
            var deletedReset = await _context.PasswordResets.FindAsync(1);
            Assert.IsNull(deletedReset);
        }

        [Test]
        public async Task DeleteResetAsync_ShouldNotDelete_WhenResetDoesNotExist()
        {
            // Act
            await _passwordResetRepository.DeleteResetAsync(3);

            // Assert
            var reset = await _context.PasswordResets.FindAsync(3);
            Assert.IsNull(reset);
        }

        [TearDown]
        public void TearDown()
        {
            // Clean up any resources if needed
            _context.Database.EnsureDeleted(); // Delete the in-memory database
            _context.Dispose(); // Dispose of the context
        }
    }
}

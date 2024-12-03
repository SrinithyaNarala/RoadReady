using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using RoadReady.Authentication;
using RoadReady.Models;
using RoadReady.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestProject1
{
    internal class ReviewRepositoryTests
    {
            private RoadReadyContext _context;
            private ReviewRepository _reviewRepository;

            [SetUp]
            public void Setup()
            {
                var options = new DbContextOptionsBuilder<RoadReadyContext>()
                    .UseInMemoryDatabase(databaseName: "TestDatabase")
                    .Options;

                _context = new RoadReadyContext(options);
                _reviewRepository = new ReviewRepository(_context);

                // Seed test data
                SeedDatabase();
            }

            private void SeedDatabase()
            {
                _context.Reviews.AddRange(
                    new Review { ReviewId = 1, CarId = 1, UserId = 1, Rating = 5, ReviewText = "Excellent car!", CreatedAt = DateTime.Now },
                    new Review { ReviewId = 2, CarId = 2, UserId = 2, Rating = 4, ReviewText = "Good experience.", CreatedAt = DateTime.Now }
                );
                _context.SaveChanges();
            }

            [TearDown]
            public void Teardown()
            {
                _context.Database.EnsureDeleted();
                _context.Dispose();
            }
        [Test]
        public async Task GetReviewByCarIdAsync_ShouldReturnCorrectReview()
        {
            // Act
            var review = await _reviewRepository.GetReviewByCarIdAsync(1);

            // Assert
            Assert.IsNotNull(review, "Expected a review but got null.");
            Assert.AreEqual(1, review.CarId, "CarId mismatch.");
            Assert.AreEqual(5, review.Rating, "Rating mismatch.");
            Assert.AreEqual("Excellent car!", review.ReviewText, "ReviewText mismatch.");
        }

        [Test]
        public async Task GetAllReviewsAsync_ShouldReturnAllReviews()
        {
            // Act
            var reviews = await _reviewRepository.GetAllReviewsAsync();

            // Assert
            Assert.AreEqual(2, reviews.Count(), "Expected two reviews.");
        }
        [Test]
        public async Task AddReviewAsync_ShouldAddNewReview()
        {
            // Arrange
            var newReview = new Review
            {
                ReviewId = 3,
                CarId = 3,
                UserId = 3,
                Rating = 4,
                ReviewText = "Nice car.",
                CreatedAt = DateTime.Now
            };

            // Act
            await _reviewRepository.AddReviewAsync(newReview);
            var addedReview = await _reviewRepository.GetReviewByCarIdAsync(3);

            // Assert
            Assert.IsNotNull(addedReview, "Expected a review but got null.");
            Assert.AreEqual("Nice car.", addedReview.ReviewText, "ReviewText mismatch.");
        }
        [Test]
        public async Task UpdateReviewAsync_ShouldUpdateExistingReview()
        {
            // Arrange
            var existingReview = await _reviewRepository.GetReviewByCarIdAsync(1);
            existingReview.Rating = 3;
            existingReview.ReviewText = "Updated review.";

            // Act
            await _reviewRepository.UpdateReviewAsync(existingReview);
            var updatedReview = await _reviewRepository.GetReviewByCarIdAsync(1);

            // Assert
            Assert.AreEqual(3, updatedReview.Rating, "Rating mismatch.");
            Assert.AreEqual("Updated review.", updatedReview.ReviewText, "ReviewText mismatch.");
        }

        [Test]
        public async Task DeleteReviewByCarIdAsync_ShouldDeleteReview()
        {
            // Act
            await _reviewRepository.DeleteReviewByCarIdAsync(1);
            var deletedReview = await _reviewRepository.GetReviewByCarIdAsync(1);

            // Assert
            Assert.IsNull(deletedReview, "Expected null but got a review.");
        }

    }
}

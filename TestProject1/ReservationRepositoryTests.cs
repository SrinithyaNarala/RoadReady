using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using RoadReady.Authentication;
using RoadReady.Models;
using RoadReady.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

[TestFixture]
public class ReservationRepositoryTests
{
    private RoadReadyContext _context;
    private ReservationRepository _reservationRepository;

    [SetUp]
    public void Setup()
    {
        // Create in-memory DbContextOptions for isolated testing
        var options = new DbContextOptionsBuilder<RoadReadyContext>()
            .UseInMemoryDatabase(databaseName: $"TestDatabase_{Guid.NewGuid()}")
            .Options;

        // Initialize DbContext
        _context = new RoadReadyContext(options);

        // Seed data
        var extras = new List<CarExtra>
        {
            new CarExtra { ExtraId = 1, Name = "GPS", Price = 10.00M },
            new CarExtra { ExtraId = 2, Name = "Baby Seat", Price = 15.00M }
        };

        var reservations = new List<Reservation>
        {
            new Reservation
            {
                ReservationId = 1,
                UserId = 1,
                CarId = 1,
                PickupDate = DateTime.Now.AddDays(1),
                DropoffDate = DateTime.Now.AddDays(3),
                TotalPrice = 100.00M,
                Status = "Active",
                Extras = new List<CarExtra> { extras[0] },
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            },
            new Reservation
            {
                ReservationId = 2,
                UserId = 2,
                CarId = 2,
                PickupDate = DateTime.Now.AddDays(5),
                DropoffDate = DateTime.Now.AddDays(7),
                TotalPrice = 200.00M,
                Status = "Cancelled",
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            }
        };

        _context.CarExtras.AddRange(extras);
        _context.Reservations.AddRange(reservations);
        _context.SaveChanges();

        // Initialize repository
        _reservationRepository = new ReservationRepository(_context);
    }

    [TearDown]
    public void Cleanup()
    {
        // Dispose of DbContext after each test
        _context.Dispose();
    }

    [Test]
    public async Task GetReservationByIdAsync_ShouldReturnReservation_WhenExists()
    {
        // Act
        var result = await _reservationRepository.GetReservationByIdAsync(1);

        // Assert
        Assert.IsNotNull(result, "Expected a reservation but got null.");
        Assert.AreEqual(1, result.ReservationId, "ReservationId mismatch.");
        Assert.AreEqual(1, result.Extras.Count, "Number of CarExtras mismatch.");
    }

    [Test]
    public async Task GetReservationByIdAsync_ShouldReturnNull_WhenNotExists()
    {
        // Act
        var result = await _reservationRepository.GetReservationByIdAsync(99);

        // Assert
        Assert.IsNull(result, "Expected null but found a reservation.");
    }

    [Test]
    public async Task GetAllReservationsAsync_ShouldReturnAllReservations()
    {
        // Act
        var result = await _reservationRepository.GetAllReservationsAsync();

        // Assert
        Assert.IsNotNull(result, "Expected a list of reservations but got null.");
        Assert.AreEqual(2, result.Count(), "Number of reservations mismatch.");
    }

    [Test]
    public async Task AddReservationAsync_ShouldAddNewReservationWithExtras()
    {
        // Arrange
        var newReservation = new Reservation
        {
            ReservationId = 3,
            UserId = 3,
            CarId = 3,
            PickupDate = DateTime.Now.AddDays(2),
            DropoffDate = DateTime.Now.AddDays(4),
            TotalPrice = 120.00M,
            Status = "Active",
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now,
            CarExtraIds = new List<int> { 1, 2 }
        };

        // Act
        await _reservationRepository.AddReservationAsync(newReservation);

        // Assert
        var result = await _reservationRepository.GetReservationByIdAsync(3);
        Assert.IsNotNull(result, "Expected the new reservation but got null.");
        Assert.AreEqual(2, result.Extras.Count, "Number of CarExtras mismatch.");
    }

    [Test]
    public async Task UpdateReservationAsync_ShouldUpdateReservationAndExtras()
    {
        // Arrange
        var reservationToUpdate = await _reservationRepository.GetReservationByIdAsync(1);
        reservationToUpdate.DropoffDate = DateTime.Now.AddDays(5);
        reservationToUpdate.CarExtraIds = new List<int> { 2 }; // Add "Baby Seat" extra

        // Act
        await _reservationRepository.UpdateReservationAsync(reservationToUpdate);

        // Assert
        var updatedReservation = await _reservationRepository.GetReservationByIdAsync(1);
        Assert.IsNotNull(updatedReservation, "Expected the updated reservation but got null.");
        Assert.AreEqual(1, updatedReservation.Extras.Count, "Number of CarExtras mismatch.");
        Assert.AreEqual("Baby Seat", updatedReservation.Extras.First().Name, "CarExtra mismatch.");
        Assert.AreEqual(reservationToUpdate.DropoffDate, updatedReservation.DropoffDate, "DropoffDate update failed.");
    }


    [Test]
    public async Task DeleteReservationAsync_ShouldRemoveReservation_WhenExists()
    {
        // Act
        await _reservationRepository.DeleteReservationAsync(1);

        // Assert
        var result = await _reservationRepository.GetReservationByIdAsync(1);
        Assert.IsNull(result, "Expected the reservation to be deleted but it still exists.");
    }

    [Test]
    public async Task DeleteReservationAsync_ShouldNotThrow_WhenNotExists()
    {
        // Act & Assert
        Assert.DoesNotThrowAsync(async () => await _reservationRepository.DeleteReservationAsync(99), "Expected no exception when deleting a non-existent reservation.");
    }
}

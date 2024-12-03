using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using RoadReady.Authentication;
using RoadReady.Models;
using RoadReady.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

[TestFixture]
public class CarExtraRepositoryTests
{
    private RoadReadyContext _context;
    private CarExtraRepository _carExtraRepository;

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
        _context.CarExtras.AddRange(
            new CarExtra { ExtraId = 1, Name = "GPS", Description = "GPS Navigation System", Price = 10.50m },
            new CarExtra { ExtraId = 2, Name = "Child Seat", Description = "Infant Car Seat", Price = 5.75m }
        );
        _context.SaveChanges();

        // Initialize repository with context
        _carExtraRepository = new CarExtraRepository(_context);
    }

    [TearDown]
    public void Cleanup()
    {
        // Dispose DbContext to clean up resources
        _context.Dispose();
    }

    [Test]
    public void GetAllCarExtras_ShouldReturnAllCarExtras()
    {
        // Act
        var result = _carExtraRepository.GetAllCarExtras();

        // Assert
        Assert.IsNotNull(result, "Expected non-null result but got null.");
        Assert.AreEqual(2, result.Count(), "Expected two car extras but found a different count.");
    }

    [Test]
    public void GetCarExtraById_ShouldReturnCarExtra_WhenDataExists()
    {
        // Act
        var result = _carExtraRepository.GetCarExtraById(1);

        // Assert
        Assert.IsNotNull(result, "Expected car extra to be found but was null.");
        Assert.AreEqual("GPS", result.Name, "Car extra name does not match.");
        Assert.AreEqual("GPS Navigation System", result.Description, "Car extra description does not match.");
    }

    [Test]
    public async Task GetCarExtrasByIdsAsync_ShouldReturnCarExtras_WhenIdsExist()
    {
        // Arrange
        var carExtraIds = new List<int> { 1, 2 };

        // Act
        var result = await _carExtraRepository.GetCarExtrasByIdsAsync(carExtraIds);

        // Assert
        Assert.IsNotNull(result, "Expected car extras to be found but was null.");
        Assert.AreEqual(2, result.Count, "Expected two car extras but found a different count.");
    }

    [Test]
    public void AddCarExtra_ShouldAddCarExtra()
    {
        // Arrange
        var newCarExtra = new CarExtra
        {
            ExtraId = 3,
            Name = "Bluetooth",
            Description = "Wireless Audio System",
            Price = 8.00m
        };

        // Act
        _carExtraRepository.AddCarExtra(newCarExtra);

        // Assert
        var result = _carExtraRepository.GetCarExtraById(3);
        Assert.IsNotNull(result, "Expected added car extra to be found but was null.");
        Assert.AreEqual("Bluetooth", result.Name, "Added car extra name does not match.");
        Assert.AreEqual("Wireless Audio System", result.Description, "Added car extra description does not match.");
    }

    [Test]
    public void UpdateCarExtra_ShouldUpdateCarExtra()
    {
        // Arrange
        var existingCarExtra = _carExtraRepository.GetCarExtraById(1);
        existingCarExtra.Name = "Updated GPS";
        existingCarExtra.Price = 12.00m;

        // Act
        _carExtraRepository.UpdateCarExtra(existingCarExtra);

        // Assert
        var result = _carExtraRepository.GetCarExtraById(1);
        Assert.AreEqual("Updated GPS", result.Name, "Updated car extra name does not match.");
        Assert.AreEqual(12.00m, result.Price, "Updated car extra price does not match.");
    }

    [Test]
    public void DeleteCarExtra_ShouldRemoveCarExtra_WhenCarExtraExists()
    {
        // Act
        _carExtraRepository.DeleteCarExtra(1);

        // Assert
        var result = _carExtraRepository.GetCarExtraById(1);
        Assert.IsNull(result, "Expected car extra to be removed but was found.");
    }
}

using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using RoadReady.Authentication;
using RoadReady.Models;
using RoadReady.Repositories;
using System.Linq;
using System.Threading.Tasks;

[TestFixture]
public class CarRepositoryTests
{
    private RoadReadyContext _context;
    private CarRepository _carRepository;

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
        _context.Cars.AddRange(
            new Car { CarId = 1, Make = "Toyota", Model = "Corolla" },
            new Car { CarId = 2, Make = "Honda", Model = "Civic" }
        );
        _context.SaveChanges();

        // Initialize repository with context
        _carRepository = new CarRepository(_context);
    }

    [Test]
    public async Task GetCarByIdAsync_ShouldReturnCar_WhenCarExists()
    {
        var result = await _carRepository.GetCarByIdAsync(1);
        Assert.IsNotNull(result, "Expected car to be found but was null.");
        Assert.AreEqual("Toyota", result.Make, "Car make does not match.");
        Assert.AreEqual("Corolla", result.Model, "Car model does not match.");
    }

    [Test]
    public async Task GetAllCarsAsync_ShouldReturnAllCars()
    {
        var result = await _carRepository.GetAllCarsAsync();
        Assert.IsNotNull(result, "Expected non-null result but got null.");
        Assert.AreEqual(2, result.Count(), "Expected two cars but found a different count.");
    }

    [Test]
    public async Task AddCarAsync_ShouldAddCar()
    {
        var newCar = new Car { CarId = 3, Make = "Ford", Model = "Focus" };
        await _carRepository.AddCarAsync(newCar);

        var result = await _carRepository.GetCarByIdAsync(3);
        Assert.IsNotNull(result, "Expected added car to be found but was null.");
        Assert.AreEqual("Ford", result.Make, "Added car make does not match.");
        Assert.AreEqual("Focus", result.Model, "Added car model does not match.");
    }

    [Test]
    public async Task UpdateCarAsync_ShouldUpdateCar()
    {
        var existingCar = await _carRepository.GetCarByIdAsync(1);
        existingCar.Make = "Toyota Updated";

        await _carRepository.UpdateCarAsync(existingCar);

        var result = await _carRepository.GetCarByIdAsync(1);
        Assert.AreEqual("Toyota Updated", result.Make, "Updated car make does not match.");
    }

    [Test]
    public async Task DeleteCarAsync_ShouldRemoveCar_WhenCarExists()
    {
        await _carRepository.DeleteCarAsync(1);

        var result = await _carRepository.GetCarByIdAsync(1);
        Assert.IsNull(result, "Expected car to be deleted but it still exists.");
    }
}

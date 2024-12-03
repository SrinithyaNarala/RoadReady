using Microsoft.EntityFrameworkCore;
using RoadReady.Authentication;
using RoadReady.Models;
using RoadReady.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class CarRepository : ICarRepository
{
    private readonly RoadReadyContext _context;

    public CarRepository(RoadReadyContext context)
    {
        _context = context;
    }

    public async Task<Car> GetCarByIdAsync(int id)
    {
        return await _context.Cars.FindAsync(id);
    }

    public async Task<IEnumerable<Car>> GetAllCarsAsync()
    {
        return await _context.Cars.ToListAsync();
    }

    public async Task AddCarAsync(Car car)
    {
        await _context.Cars.AddAsync(car);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateCarAsync(Car car)
    {
        _context.Cars.Update(car);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteCarAsync(int id)
    {
        var car = await GetCarByIdAsync(id);
        if (car != null)
        {
            _context.Cars.Remove(car);
            await _context.SaveChangesAsync();
        }
    }
}
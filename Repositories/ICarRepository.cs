using RoadReady.Models;

namespace RoadReady.Repositories
{
    public interface ICarRepository
    {
        Task<Car> GetCarByIdAsync(int id);
        Task<IEnumerable<Car>> GetAllCarsAsync();
        Task AddCarAsync(Car car);
        Task UpdateCarAsync(Car car);
        Task DeleteCarAsync(int id);
    }
}
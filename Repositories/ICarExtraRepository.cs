using RoadReady.Models;

namespace RoadReady.Repositories
{
    public interface ICarExtraRepository
    {
        Task<List<CarExtra>> GetCarExtrasByIdsAsync(List<int> carExtraIds);
        IEnumerable<CarExtra> GetAllCarExtras();
        CarExtra GetCarExtraById(int extraId);
        void AddCarExtra(CarExtra carExtra);
        void UpdateCarExtra(CarExtra carExtra);
        void DeleteCarExtra(int extraId);
    }
}
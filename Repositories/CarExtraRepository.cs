using Microsoft.EntityFrameworkCore;
using RoadReady.Authentication;
using RoadReady.Models;

namespace RoadReady.Repositories
{
    public class CarExtraRepository : ICarExtraRepository
    {
        private readonly RoadReadyContext _context;

        public CarExtraRepository(RoadReadyContext context)
        {
            _context = context;
        }

        public IEnumerable<CarExtra> GetAllCarExtras()
        {
            return _context.CarExtras.ToList();
        }

        public CarExtra GetCarExtraById(int extraId)
        {
            return _context.CarExtras.Find(extraId);

        }
        public async Task<List<CarExtra>> GetCarExtrasByIdsAsync(List<int> carExtraIds)
        {
            return await _context.CarExtras
                .Where(ce => carExtraIds.Contains(ce.ExtraId))
                .ToListAsync();
        }

        public void AddCarExtra(CarExtra carExtra)
        {
            _context.CarExtras.Add(carExtra);
            _context.SaveChanges();
        }

        public void UpdateCarExtra(CarExtra carExtra)
        {
            _context.CarExtras.Update(carExtra);
            _context.SaveChanges();
        }

        public void DeleteCarExtra(int extraId)
        {
            var carExtra = _context.CarExtras.Find(extraId);
            if (carExtra != null)
            {
                _context.CarExtras.Remove(carExtra);
                _context.SaveChanges();
            }
        }
    }
}
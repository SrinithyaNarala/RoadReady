using Microsoft.EntityFrameworkCore;
using RoadReady.Authentication;
using RoadReady.Models;
using RoadReady.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class ReviewRepository : IReviewRepository
{
    private readonly RoadReadyContext _context;

    public ReviewRepository(RoadReadyContext context)
    {
        _context = context;
    }

    public async Task<Review> GetReviewByCarIdAsync(int carId)
    {
        return await _context.Reviews.FirstOrDefaultAsync(r => r.CarId == carId);
    }

    public async Task<IEnumerable<Review>> GetAllReviewsAsync()
    {
        return await _context.Reviews.ToListAsync();
    }

    public async Task AddReviewAsync(Review review)
    {
        await _context.Reviews.AddAsync(review);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateReviewAsync(Review review)
    {
        // Check for an already tracked instance of the Review entity
        var trackedEntity = _context.ChangeTracker.Entries<Review>()
                                    .FirstOrDefault(e => e.Entity.ReviewId == review.ReviewId);

        if (trackedEntity != null)
        {
            // Detach the tracked entity to avoid conflict
            trackedEntity.State = EntityState.Detached;
        }

        // Attach and update the review entity
        _context.Reviews.Update(review);

        // Save changes to the database
        await _context.SaveChangesAsync();
    }



    public async Task DeleteReviewByCarIdAsync(int carId)
    {
        var review = await GetReviewByCarIdAsync(carId);
        if (review != null)
        {
            _context.Reviews.Remove(review);
            await _context.SaveChangesAsync();
        }
    }





}
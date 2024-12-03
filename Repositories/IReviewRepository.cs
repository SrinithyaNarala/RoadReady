using RoadReady.Models;

namespace RoadReady.Repositories
{
    public interface IReviewRepository
    {
        Task<Review> GetReviewByCarIdAsync(int id);
        Task<IEnumerable<Review>> GetAllReviewsAsync();
        Task AddReviewAsync(Review review);
        Task UpdateReviewAsync(Review review);
        Task DeleteReviewByCarIdAsync(int id);

    }
}
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RoadReady.Models;
using RoadReady.Models.DTO;
using RoadReady.Repositories;

namespace RoadReady.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewsController : ControllerBase
    {
        private readonly IReviewRepository _reviewRepository;
        private readonly ILogger<ReviewsController> _logger;
        private readonly IMapper _mapper;

        public ReviewsController(IReviewRepository reviewRepository, ILogger<ReviewsController> logger, IMapper mapper)
        {
            _reviewRepository = reviewRepository;
            _logger = logger;
            _mapper = mapper;
        }

        // GET: api/Reviews
        [HttpGet]
        [Authorize(Roles = "Admin,Customer,Agent")]
        public async Task<ActionResult<IEnumerable<ReviewsDTO>>> GetAllReviews()
        {
            try
            {
                var reviews = await _reviewRepository.GetAllReviewsAsync();
                if (reviews == null || !reviews.Any())
                {
                    return NotFound("No reviews found.");
                }

                var reviewsDto = _mapper.Map<IEnumerable<ReviewsDTO>>(reviews);
                return Ok(reviewsDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving all reviews.");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // GET: api/Reviews/ByCar/{carId}
        [HttpGet("ByCar/{carId}")]
        [Authorize(Roles = "Admin,Customer,Agent")]
        public async Task<ActionResult<ReviewsDTO>> GetReviewByCarId(int carId)
        {
            try
            {
                var review = await _reviewRepository.GetReviewByCarIdAsync(carId);
                if (review == null)
                {
                    return NotFound($"Review for Car ID {carId} not found.");
                }

                var reviewDto = _mapper.Map<ReviewsDTO>(review);
                return Ok(reviewDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while retrieving the review for Car ID {carId}.");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // POST: api/Reviews
        [HttpPost]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> AddReview([FromBody] ReviewsDTO reviewDto)
        {
            if (reviewDto == null)
            {
                return BadRequest("Review data is required.");
            }

            if (reviewDto.CarId <= 0)
            {
                return BadRequest("A valid CarId is required to add a review.");
            }

            try
            {
                var review = _mapper.Map<Review>(reviewDto);
                await _reviewRepository.AddReviewAsync(review);
                var createdReviewDto = _mapper.Map<ReviewsDTO>(review);

                return CreatedAtAction(nameof(GetReviewByCarId), new { carId = review.CarId }, createdReviewDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding a new review.");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // PUT: api/Reviews/ByCar/{carId}
        [HttpPut("ByCar/{carId}")]
        [Authorize(Roles = "Admin,Customer")]
        public async Task<IActionResult> UpdateReview(int carId, [FromBody] ReviewsDTO reviewDto)
        {
            if (reviewDto == null)
            {
                return BadRequest("Review data is required.");
            }

            if (carId != reviewDto.CarId)
            {
                return BadRequest("Car ID mismatch.");
            }

            try
            {
                var existingReview = await _reviewRepository.GetReviewByCarIdAsync(carId);

                if (existingReview == null)
                {
                    return NotFound($"Review for Car ID {carId} not found.");
                }

                var updatedReview = _mapper.Map(reviewDto, existingReview);
                await _reviewRepository.UpdateReviewAsync(updatedReview);

                return Ok(new { message = $"Review for Car ID {carId} has been updated." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while updating the review for Car ID {carId}.");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}




/*[HttpDelete("ByCar/{carId}")]
[Authorize(Roles = "Admin")]
public async Task<IActionResult> DeleteReview(int carId)
{
    try
    {
        // Get the review associated with the carId
        var review = await _reviewRepository.GetReviewByCarIdAsync(carId);
        if (review == null)
        {
            return NotFound($"Review for Car ID {carId} not found.");
        }

        // Delete the review associated with the carId
        await _reviewRepository.DeleteReviewByCarIdAsync(carId);
        return Ok(new { message = $"Review for Car ID {carId} has been deleted." });
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, $"An error occurred while deleting the review for Car ID {carId}.");
        return StatusCode(500, $"Internal server error: {ex.Message}");
    }
}*/


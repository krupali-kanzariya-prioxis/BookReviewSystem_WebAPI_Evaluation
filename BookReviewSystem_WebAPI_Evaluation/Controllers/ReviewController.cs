using Microsoft.AspNetCore.Mvc;
using Review.Common;
using Review.Models.CommonModel;
using Review.Models.RequestModel;
using Review.Services.Repository.Interfaces;

namespace BookReviewSystem_WebAPI_Evaluation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewController : BaseController
    {
        private readonly IReviewRepository _reviewRepository;
        private readonly ILogger<ReviewController> _logger;

        public ReviewController(IReviewRepository reviewRepository, ILogger<ReviewController> logger)
        {
            _reviewRepository = reviewRepository;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<Page>> GetAllReviews([FromQuery] SearchRequestModel model)
        {
            try
            {
                var parameters = FillParamesFromModel(model);
                var reviews = FillParamesFromModel(model);
                var list = await _reviewRepository.GetReviewsAsync(parameters);
                return Ok(BindSearchResult(list, model, "Review List"));
            }
            catch (Exception ex)
            {
                return StatusCode(400, $"Bad Request Error: {ex.Message}");
            }
        }

        [HttpGet("{reviewSid}")]
        public async Task<ActionResult<ReviewResponseModel>> GetReviewById(string reviewSid)
        {
            try
            {
                var review = await _reviewRepository.GetReviewByIdAsync(reviewSid);
                if (review == null)
                    throw new HttpStatusCodeException(400, $"Review with SID '{reviewSid}' not found.");
                return Ok(review);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        [HttpDelete("{reviewSid}")]
        public async Task<IActionResult> DeleteReview(string reviewSid)
        {
            try
            {
                var deleted = await _reviewRepository.DeleteReviewAsync(reviewSid);
                if (!deleted)
                    return NotFound($"Review with SID '{reviewSid}' not found.");
                return Ok($"Review with SID '{reviewSid}' deleted successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        [HttpPost("AddOrUpdateReview/{reviewSid?}")]
        public async Task<ActionResult<ReviewResponseModel>> AddOrUpdateReview(
            [FromRoute] string? reviewSid,
            [FromBody] ReviewRequestModel review)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                if (string.IsNullOrEmpty(reviewSid))
                {
                    var response = await _reviewRepository.AddReviewAsync(review);
                    return Ok(response);
                }
                else
                {
                    var response = await _reviewRepository.UpdateReviewAsync(reviewSid, review);
                    if (response == null)
                        return NotFound($"Review with SID '{reviewSid}' not found.");

                    return Ok(response);
                }
            }
            catch (HttpStatusCodeException ex) when (ex.StatusCode == 500)
            {
                return StatusCode(500, "Internal Server Error while processing the request.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Unexpected error: {ex.Message}");
            }
        }
        [HttpGet("rating/{rating}")]
        public async Task<ActionResult<List<ReviewResponseModel>>> GetReviewsByRating(int rating)
        {
            try
            {
                var reviews = await _reviewRepository.GetReviewsByRatingAsync(rating);
                if (reviews == null || reviews.Count == 0)
                    return NotFound($"No reviews found with Rating '{rating}'.");
                return Ok(reviews);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

    }
}

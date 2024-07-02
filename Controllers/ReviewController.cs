using Guide_Me.DTO;
using Guide_Me.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Guide_Me.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ReviewController : ControllerBase
    {
        private readonly IReviewsService _reviewService;

        public ReviewController(IReviewsService reviewService)
        {
            _reviewService = reviewService;
        }

        [HttpGet("GetReviews")]
        public IActionResult GetReviews(string placeName, string touristName)
        {
            var reviews = _reviewService.GetReviewOnPlace(placeName, touristName);
            if (reviews != null)
            {
                return Ok(reviews);
            }
            else
            {
                return BadRequest("No Reviews Found");
            }
        }

        [HttpPost("AddReview")]
        public IActionResult ReviewPlace(ReviewPlaceDto reviewPlaceDto)
        {
            if (_reviewService.AddReviewOnPlace(reviewPlaceDto))
            {
                return Ok("Review Saved");
            }
            else
            {
                return BadRequest("Error in saving review");
            }
        }
    }
}

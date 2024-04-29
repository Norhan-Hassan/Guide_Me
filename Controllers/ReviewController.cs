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
        public IActionResult GetReviews(string placeName)
        {
            if (_reviewService.GetReviewOnPlace(placeName) != null)
            {
                
                return Ok(_reviewService.GetReviewOnPlace(placeName));
            }
            else
            {
                return BadRequest("No Reviews Found");
            }
        }


        [HttpPost("AddReview")]
        public IActionResult ReviewPlace(ReviewPlaceDto reviewPlaceDto)
        {
            if (_reviewService.AddReviewOnPlace(reviewPlaceDto) == true)
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

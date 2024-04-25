using Guide_Me.DTO;
using Guide_Me.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Guide_Me.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RatingController : ControllerBase
    {
        private readonly IRatingService _ratingService;

        public RatingController(IRatingService ratingService)
        {
            _ratingService = ratingService;
        }
      
        [HttpPost("RatePlace")]
        public IActionResult RatePlace(RatePlaceDto request)
        {
            try
            {
                if(_ratingService.RatePlace(request)==true)
                {
                    return Ok("Place Rated Successfully.");
                }
                else
                { return BadRequest("place or tourist not found"); }
            }
            catch (Exception ex)
            {

                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
        [HttpGet("{placeName}/OverAllRating")]
        public IActionResult GetMediaByPlace(string placeName)
        {
            int rate= _ratingService.GetOverAllRateOfPlace(placeName);
            return Ok(rate);
        }
    }
}

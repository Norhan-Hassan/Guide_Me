using Guide_Me.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Guide_Me.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RecommendationController : ControllerBase
    {
        private readonly IRecommendationService _recommendationService;

        public RecommendationController(IRecommendationService recommendationService)
        {
            _recommendationService = recommendationService;

        }

        [HttpGet("GetRecommendations")]
        public IActionResult GetRecommendationByPrefrences(string touristName, string cityName,double Latitude, double Longitude)
        {
            if (_recommendationService.GetRecommendation(touristName, cityName, Latitude, Longitude) != null)
            {

                return Ok(_recommendationService.GetRecommendation(touristName,cityName, Latitude, Longitude));
            }
            else
            {
                return BadRequest("No Recommendation is Found");
            }
        }

    }
}

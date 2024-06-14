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

        [HttpGet("GetRecommendationByPreference")]
        public IActionResult GetRecommendationByPrefrences(string touristName, string cityName)
        {
            if (_recommendationService.GetRecommendationByPreferences(touristName,cityName) != null)
            {

                return Ok(_recommendationService.GetRecommendationByPreferences(touristName,cityName));
            }
            else
            {
                return BadRequest("No Recommendation is Found");
            }
        }

    }
}

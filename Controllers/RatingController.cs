using Guide_Me.DTO;
using Guide_Me.Models;
using Guide_Me.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Guide_Me.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
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
                if (_ratingService.RatePlace(request) == true)
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
        public IActionResult GetOverallRating(string placeName)
        {
            int rate = _ratingService.GetOverAllRateOfPlace(placeName);
            return Ok(rate);
        }

        [HttpGet("{RateNumber}/Rating/Suggestion")]
        public IActionResult GetSggestions(int RateNumber)
        {
            List<string> suggestions = _ratingService.GetSuggestionsBasedOnRating(RateNumber).ToList();
            if (suggestions.Count == 0)
            {
                return BadRequest("No Suggestions");
            }
            else
            {
                return Ok(suggestions);
            }
        }

        [HttpPost("{Suggestion}/Rating/Suggestion")]
        public IActionResult AddSuggestionChosenByTourist(string Suggestion, RatePlaceDto request)
        {
            if(_ratingService.AddSuggestionChoosen(Suggestion, request)==true)
                return Ok("Suggestion Saved");
            else
            { return BadRequest("Error in add suggestion rate"); }
        }
    }
}

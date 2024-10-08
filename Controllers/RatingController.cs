﻿using Guide_Me.DTO;
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
        [HttpGet("{placeName}/{touristName}/OverAllRating")]
        public IActionResult GetOverallRating(string placeName, string touristName)
        {
            int rate = _ratingService.GetOverAllRateOfPlace(placeName,touristName);
            return Ok(rate);
        }

        [HttpGet("{RateNumber}/{touristName}/Rating/Suggestion")]
        public IActionResult GetSggestions(int RateNumber, string touristName)
        {
            List<string> suggestions = _ratingService.GetSuggestionsBasedOnRating(RateNumber, touristName).ToList();
            if (suggestions.Count == 0)
            {
                return BadRequest("No Suggestions");
            }
            else
            {
                return Ok(suggestions);
            }
        }

        [HttpPost("/Rating/Suggestion")]
        public IActionResult AddSuggestionChosenByTourist(RatePlaceWithSuggDto request)
        {
            if(_ratingService.AddSuggestionChoosen(request)==true)
                return Ok("Suggestion Saved");
            else
            { return BadRequest("Error in add suggestion rate"); }
        }
        [HttpGet("/Rating/GetLatestRate")]
        public IActionResult GetRate(string TouristName, string PlaceName)
        {
           int rateNum = _ratingService.GetLatestRateOfToursit(TouristName, PlaceName);
           if (rateNum!=-1)
           {
                return Ok(rateNum);
           }
           else
            {
                return BadRequest("Tourist or place Not Found");
            }
        }
    }
}

using Guide_Me.DTO;
using Guide_Me.Models;
using Guide_Me.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Guide_Me.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SuggestionPlacesController : ControllerBase
    {
        private readonly ISuggestionplacebyuserService _suggestionService;

        public SuggestionPlacesController(ISuggestionplacebyuserService suggestionService)
        {
            _suggestionService = suggestionService;
        }

        [HttpPost]
        public async Task<IActionResult> SubmitSuggestion(string placeName, string? address, double? latitude, double? longitude, string touristName)
        {
            try
            {
                // Ensure that either address or location (latitude and longitude) is provided
                if (string.IsNullOrEmpty(address) && (!latitude.HasValue || !longitude.HasValue))
                {
                    return BadRequest("Either address or location (latitude and longitude) must be provided.");
                }

                await _suggestionService.SubmitSuggestion(placeName, address, latitude, longitude, touristName);
                return Ok("Suggestion submitted successfully.");
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(500, "An error occurred while processing the request.");
            }
        }
    }
}

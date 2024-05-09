using Guide_Me.DTO;
using Guide_Me.Models; 
using Guide_Me.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Guide_Me.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SuggestionPlacesController : ControllerBase
    {
        private readonly ISuggestionplacebyuserService _suggestionService;

        public SuggestionPlacesController(ISuggestionplacebyuserService suggestionService)
        {
            _suggestionService = suggestionService;
        }

        [HttpPost]
        public async Task<IActionResult> SubmitSuggestion( string placeName,  string address, string touristName)
        {
            try
            {
                await _suggestionService.SubmitSuggestionAsync(placeName, address, touristName);
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

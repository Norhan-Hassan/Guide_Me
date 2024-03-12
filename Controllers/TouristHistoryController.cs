using Guide_Me.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Guide_Me.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TouristHistoryController : ControllerBase
    {
        private readonly IHistoryService _historyService;

        public TouristHistoryController(IHistoryService historyService)
        {
            _historyService = historyService;
        }

        // POST: api/HistoryApi
        [HttpPost]
        public IActionResult UpdatePlaceHistory(int placeId, int touristId)
        {
            try
            {
                
                DateTime currentDate = DateTime.Now;
                _historyService.UpdatePlaceHistory(placeId, touristId, currentDate);
                return Ok("Place history updated successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
    }
}


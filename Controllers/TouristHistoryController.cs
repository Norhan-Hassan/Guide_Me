using Guide_Me.Models;
using Guide_Me.Services;
using Microsoft.AspNetCore.Mvc;
using System;

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

        
        [HttpPost]
        public IActionResult UpdatePlaceHistory(int placeId, string touristId)
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

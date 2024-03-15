using Guide_Me.Models;
using Guide_Me.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;

namespace Guide_Me.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class TouristHistoryController : ControllerBase
    {
        private readonly ILogger<TouristHistoryController> _logger;
        private readonly IHistoryService _historyService;

        public TouristHistoryController(IHistoryService historyService, ILogger<TouristHistoryController> logger)
        {
            _historyService = historyService;
            _logger = logger;
        }

        
        [HttpPost]
        public IActionResult UpdatePlaceHistory(int placeId, string touristname)
        {
            try
            {
                
                DateTime currentDate = DateTime.Now;

                
                _historyService.UpdatePlaceHistory(placeId, touristname, currentDate);

                
                return Ok("Place history updated successfully.");
            }
            catch (Exception ex)
            {
                
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }


        [HttpGet("{touristname}")]
        public IActionResult GetTouristHistory(string touristname)
        {
            try
            {
                _logger.LogInformation($"Received request for tourist history for username: {touristname}");

                var userHistory = _historyService.GetTouristHistory(touristname);
                return Ok(userHistory);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while fetching tourist history for username {touristname}: {ex}");
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
    }
}

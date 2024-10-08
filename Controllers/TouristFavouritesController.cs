﻿using Guide_Me.DTO;
using Guide_Me.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Guide_Me.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]

    public class TouristFavouritesController : ControllerBase
    {
        private readonly ILogger<TouristFavouritesController> _logger;
        private readonly IFavoritePlaceService _IFavoritePlaceService;

        public TouristFavouritesController(IFavoritePlaceService IFavoritePlaceService, ILogger<TouristFavouritesController> logger)
        {
            _IFavoritePlaceService = IFavoritePlaceService;
            _logger = logger;
        }


        [HttpPost("AddFavoritePlace")]
        public IActionResult AddFavoritePlace(FavouritePlacesDto request)
        {
            try
            {
                _IFavoritePlaceService.MarkFavoritePlace(request);
                return Ok("Favorite place added successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpPost("RemoveFavoritePlace")]
        public IActionResult RemoveFavoritePlace(FavouritePlacesDto request)
        {
            try
            {
                _IFavoritePlaceService.MarkFavoritePlace(request);
                return Ok("Favorite place removed successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpPost("GetTouristFavoritePlaces")]
        public IActionResult GetFavoritePlaces(string touristname)
        {
            try
            {
                _logger.LogInformation($"Received request for tourist FavoritePlaces for username: {touristname}");

                var FavoritePlaces = _IFavoritePlaceService.GetAllFavoritesByTourist(touristname);
                return Ok(FavoritePlaces);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while fetching tourist history for username {touristname}: {ex}");
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

    }
}
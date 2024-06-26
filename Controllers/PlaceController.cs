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
    public class PlaceController : ControllerBase
    {
        private readonly IPlaceService _placeService;

        public PlaceController(IPlaceService placeService)
        {
            _placeService = placeService;
        }

        [HttpGet("{CityName}/{TouristName}/Allplaces")]
        public IActionResult GetPlacesByCity(string CityName, string TouristName)
        {
            var places = _placeService.GetPlaces(CityName, TouristName);
            if (places == null)
            {
                return NotFound();
            }

            return Ok(places);
        }
        [HttpGet("{placeName}/places/items")]
        public IActionResult GetItemsByPlace(string placeName)
        {
            var placeItems = _placeService.GetPlaceItems(placeName);
            if (placeItems == null)
            {
                return NotFound();
            }

            return Ok(placeItems);
        }

        [HttpGet("{placeName}/places/location")]
        public IActionResult PostLocationByName(string placeName)
        {
            try
            {
                var placeLocation = _placeService.GetLocation(placeName);
                return Ok(placeLocation);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{placeName}/places/media")]
        public IActionResult GetMediaByPlace(string placeName)
        {
            var placeMedia = _placeService.GetPlaceMedia(placeName);
            if (placeMedia == null)
            {
                return NotFound();
            }

            return Ok(placeMedia);
        }
       
        [HttpGet]
        [Route("{placeName}/{cityName}/SearchPlace")]
        public IActionResult SearchPlace(string placeName , string cityName)
        {
            var place = _placeService.SerachPlace(placeName, cityName);
            if(place == null)
            {
                return NotFound($"Place with name '{placeName}' is not found.");
            }

            return Ok(place);
       }

    }
}

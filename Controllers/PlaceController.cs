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

        public async Task<IActionResult> GetPlaces(string CityName, string TouristName)
        {

            var places = _placeService.GetPlaces(CityName, TouristName);
            if (places == null)
            {
                return NotFound();
            }

            return Ok(places);
        }
        [HttpGet("{placeName}/{touristName}/places/items")]
        public IActionResult GetItemsByPlace(string placeName,string touristName)
        {
            var placeItems = _placeService.GetPlaceItems(placeName, touristName);
            if (placeItems == null)
            {
                return NotFound();
            }

            return Ok(placeItems);
        }

        [HttpGet("{placeName}/{touristName}/places/location")]
        public IActionResult PostLocationByName(string placeName, string touristName)
        {
            try
            {
                var placeLocation = _placeService.GetLocation(placeName,touristName);
                return Ok(placeLocation);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{placeName}/{touristName}/places/media")]
        public IActionResult GetMediaByPlace(string placeName, string touristName)
        {
            var placeMedia = _placeService.GetPlaceMedia(placeName,touristName);
            if (placeMedia == null)
            {
                return NotFound();
            }

            return Ok(placeMedia);
        }

        [HttpGet]
        [Route("{placeName}/{cityName}/{touristName}/SearchPlace")]
        public IActionResult SearchPlace(string placeName, string cityName, string touristName)
        {
            var place = _placeService.SerachPlace(placeName, cityName, touristName);
            if (place == null)
            {
                return NotFound($"Place with name '{placeName}' is not found.");
            }

            return Ok(place);
        }

    }
}

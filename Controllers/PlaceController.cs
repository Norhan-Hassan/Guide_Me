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

        [HttpGet("{CityName}/Allplaces")]
        public IActionResult GetPlacesByCity(string CityName)
        {
            var places = _placeService.GetPlaces(CityName);
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
       
        [HttpPost("{placeName}/places/location")]
        public async Task<IActionResult> PostLocationByName(string placeName, double latitude, double longitude)
        {
            try
            {
                await _placeService.PostLocationAsync(placeName, latitude, longitude);
                return Ok("Location updated successfully.");
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

    }
}

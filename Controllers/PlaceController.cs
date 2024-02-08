using Guide_Me.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Guide_Me.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlaceController : ControllerBase
    {
        private readonly IPlaceService _placeService;

        public PlaceController(IPlaceService placeService)
        {
            _placeService = placeService;
        }

        [HttpGet("{cityName}/places")]
        public IActionResult GetPlacesByCity(string cityName)
        {
            var places = _placeService.GetPlaces(cityName);
            if (places == null)
            {
                return NotFound(); 
            }

            return Ok(places);
        }
    }
}

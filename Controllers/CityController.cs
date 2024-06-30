using Guide_Me.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace Guide_Me.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CityController : ControllerBase
    {
        private readonly ICityService _cityService;

        public CityController(ICityService cityService)
        {
            _cityService = cityService;
        }

        [HttpGet]
        [Route("AllCities/{touristName}")]
        public IActionResult GetAllCities(string touristName)
        {
            var cities = _cityService.GetAllCities(touristName);
            if(cities != null)
            {
                return Ok(cities);
            }
            else
            {
                return BadRequest($"Tourist Name {touristName} is not found");
            }
            
        }

        [HttpGet]
        [Route("SearchCity/{cityName}/{touristName}")]
        public IActionResult SearchCity(string cityName, string touristName)
        {
            var city = _cityService.GetCityByName(cityName, touristName);
            if (city != null)
            {
                return Ok(city);
            }
            return NotFound($"City with name '{cityName}' not found.");
        }

    }
}

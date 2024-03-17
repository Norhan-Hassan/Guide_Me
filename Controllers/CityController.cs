using Guide_Me.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Guide_Me.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class CityController : ControllerBase
    {
        private readonly ICityService _cityService;

        public CityController(ICityService cityService)
        {
            _cityService = cityService;
        }

        [HttpGet]
        [Route("AllCities")]
        public IActionResult GetAllCities()
        {
            var cities = _cityService.GetAllCities();
            return Ok(cities);
        }
        [HttpGet]
        [Route("SearchCity/{cityName}")]
        public IActionResult SearchCity(string cityName)
        {
            var city = _cityService.GetAllCities().FirstOrDefault(c => c.Name.ToLower() == cityName.ToLower());
            if (city != null)
            {
                return Ok(city);
            }
            return NotFound($"City with name '{cityName}' not found.");
        }
    }
}


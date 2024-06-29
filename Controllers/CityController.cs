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
        [Route("AllCities")]
        public IActionResult GetAllCities([FromQuery] string targetLanguage)
        {
            if (string.IsNullOrEmpty(targetLanguage))
            {
                // Set a default language if necessary
                targetLanguage = "en"; // English as default
            }

            var cities = _cityService.GetAllCities(targetLanguage);
            return Ok(cities);
        }

        [HttpGet]
        [Route("SearchCity/{cityName}")]
        public IActionResult SearchCity(string cityName, [FromQuery] string targetLanguage)
        {
            if (string.IsNullOrEmpty(targetLanguage))
            {
                // Set a default language if necessary
                targetLanguage = "en"; // English as default
            }

            var city = _cityService.GetCityByName(cityName, targetLanguage);
            if (city != null)
            {
                return Ok(city);
            }
            return NotFound($"City with name '{cityName}' not found.");
        }

    }
}

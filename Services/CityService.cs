using Guide_Me.DTO; // Assuming CityDto is defined here
using Guide_Me.Models;
using Guide_Me.Services;
using System.Collections.Generic;
using System.Linq;

namespace Guide_Me.Services
{
    public class CityService : ICityService
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IBlobStorageService _blobStorageService;
        private readonly ITranslationService _translationService; 

        public CityService(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor,
                           IBlobStorageService blobStorageService, ITranslationService translationService)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _blobStorageService = blobStorageService;
            _translationService = translationService;
        }

        public List<CityDto> GetAllCities(string touristName)
        {
            var cities = _context.Cities.ToList();
            List<CityDto> cityDtos = new List<CityDto>();
            var tourist = _context.Tourist.FirstOrDefault(t => t.UserName == touristName);
            if (tourist == null)
            {
                return null;
            }

            var targetLanguage = tourist.Language;

            foreach (var city in cities)
            {
                string cityNameToUse = city.CityName;
                if (targetLanguage != "en")
                {
                    cityNameToUse = _translationService.TranslateTextResultASync(city.CityName, targetLanguage);
                }

                CityDto cityDto = new CityDto
                {
                    Id = city.Id,
                    Name = cityNameToUse,
                    CityImage = GetMediaUrl(city.CityImage),
                };

                cityDtos.Add(cityDto);
            }

            return cityDtos;
        }

        public CityDto GetCityByName(string cityName, string touristName)
        {
            var city = _context.Cities.FirstOrDefault(c => c.CityName.ToLower() == cityName.ToLower());
            if (city == null)
            {
                return null;
            }
            var tourist = _context.Tourist.FirstOrDefault(t => t.UserName == touristName);
            if (tourist == null)
            {
                return null;
            }

            var targetLanguage = tourist.Language;

            string cityNameToUse = cityName;
            if (targetLanguage != "en")
            {
                cityNameToUse = _translationService.TranslateTextResultASync(city.CityName, targetLanguage);
            }

            CityDto cityDto = new CityDto
            {
                Id = city.Id,
                Name = cityNameToUse,
                CityImage = GetMediaUrl(city.CityImage),
            };

            return cityDto;
        }

        private string GetMediaUrl(string cityImage)
        {
            var request = _httpContextAccessor.HttpContext.Request;
            var baseUrl = $"{request.Scheme}://{request.Host}";
            return $"{baseUrl}/{cityImage}";
        }
        private string GetBlobUrlmedia(string CityImage)
        {
            // Replace this with actual container and blob name based on your storage structure
            string containerName = "firstcontainer";
            // Assuming CityImage is the blob name or a unique identifier for the blob
            string blobName = CityImage;
            // Call BlobStorageService to get the blob URL
            return _blobStorageService.GetBlobUrl(containerName, blobName);
        }

    }
}

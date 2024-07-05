using Guide_Me.DTO;
using Guide_Me.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using static System.Net.Mime.MediaTypeNames;

namespace Guide_Me.Services
{
    public class RecommendationService : IRecommendationService
    {
        private readonly ApplicationDbContext _context;
        private readonly ITouristService _touristService;
        private readonly IPlaceService _placeService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IBlobStorageService _blobStorageService;
        private readonly ITranslationService _translationService;
        private readonly ILogger<RecommendationService> _logger;

        public RecommendationService(ApplicationDbContext context,
            ITouristService touristService,
            IPlaceService placeService,
            IHttpContextAccessor httpContextAccessor,
            IBlobStorageService blobStorageService,
            ITranslationService translationService,
            ILogger<RecommendationService> logger)
        {
            _context = context;
            _touristService = touristService;
            _placeService = placeService;
            _httpContextAccessor = httpContextAccessor;
            _blobStorageService = blobStorageService;
            _translationService = translationService;
            _logger = logger;
        }

        public List<PlaceRecommendationDto> GetRecommendation(string touristName, string cityName, string placeName)
        {
            try
            {
               
                // Retrieve the tourist's details
                var tourist = _context.Tourist.FirstOrDefault(t => t.UserName == touristName);
                if (tourist == null)
                {
                    
                    return null; // Tourist not found
                }

                var preferredLanguage = tourist.Language;
            

                // Translate the place name and city name to English if the preferred language is not English
                string cityNameToSearch = cityName;
                string placeNameToSearch = placeName;
                if (preferredLanguage != "en")
                {
                    cityNameToSearch = _translationService.TranslateTextResultASync(cityName, "en");
                    placeNameToSearch = _translationService.TranslateTextResultASync(placeName, "en");
                  
                }

                string touristID = _touristService.GetUserIdByUsername(touristName);
                int placeId = _placeService.GetPlaceIdByPlaceName(placeNameToSearch);

                if (!string.IsNullOrEmpty(touristID) && placeId != 0)
                {
                    // Retrieve favorite places of the tourist
                    var favPlaces = _context.Favorites
                                            .Where(f => f.TouristID == touristID && f.Place.City.CityName == cityNameToSearch)
                                            .Select(f => f.Place)
                                            .ToList();
                  

                    // Retrieve historical places visited by the tourist
                    var histPlaces = _context.histories
                                                .Where(h => h.TouristId == touristID && h.Place.City.CityName == cityNameToSearch)
                                                .Select(h => h.Place)
                                                .ToList();
                    

                    // Retrieve latitude and longitude of the requested place
                    double Latitude = _context.Places.Where(p => p.Id == placeId).Select(p => p.latitude).FirstOrDefault();
                    double Longitude = _context.Places.Where(p => p.Id == placeId).Select(p => p.longitude).FirstOrDefault();

                    // Retrieve nearest places excluding the requested place
                    var nearestPlaces = _context.Places
                                                .Where(p => p.City.CityName == cityNameToSearch && p.PlaceName != placeNameToSearch)
                                                .Include(p => p.PlaceMedias)
                                                .Select(p => new
                                                {
                                                    Place = p,
                                                    Distance = 6371 * Math.Acos(
                                                        Math.Cos(Latitude * Math.PI / 180) * Math.Cos(p.latitude * Math.PI / 180) *
                                                        Math.Cos((p.longitude - Longitude) * Math.PI / 180) +
                                                        Math.Sin(Latitude * Math.PI / 180) * Math.Sin(p.latitude * Math.PI / 180))
                                                })
                                                .OrderBy(p => p.Distance)
                                                .Take(3)
                                                .ToList()
                                                .Select(p => new PlaceRecommendationDto
                                                {
                                                    PlaceName = preferredLanguage != "en"
                                                                ? _translationService.TranslateTextResultASync(p.Place.PlaceName, preferredLanguage)
                                                                : p.Place.PlaceName,
                                                    Image = GetPlaceImageUrl(p.Place),
                                                    Rate = _context.Rating
                                                                .Where(r => r.PlaceId == p.Place.Id)
                                                                .Select(r => r.Rate)
                                                                .FirstOrDefault()
                                                })
                                                .ToList();

                    // Combine favorite and historical places
                    var commonPlaces = favPlaces.Concat(histPlaces).ToList();

                    // Determine the most frequent category among common places
                    if (commonPlaces.Any())
                    {
                        var categoryCounts = commonPlaces
                                                .GroupBy(p => p.Category)
                                                .Select(g => new { Category = g.Key, Count = g.Count() })
                                                .OrderByDescending(g => g.Count)
                                                .ToList();

                        var mostFrequentCategory = categoryCounts.FirstOrDefault()?.Category?.ToLower();

                        if (!string.IsNullOrEmpty(mostFrequentCategory))
                        {
                            // Retrieve all places in the city with the most frequent category
                            var allPlacesInCity = _context.Places
                                                            .Include(p => p.PlaceMedias)
                                                            .Where(p => p.City.CityName == cityNameToSearch && p.Category.ToLower() == mostFrequentCategory)
                                                            .ToList();

                            // Select recommended places not in favorites or history and not the requested place
                            var recommendedPlaces = allPlacesInCity
                                                        .Where(place => !favPlaces.Contains(place) && !histPlaces.Contains(place) && place.PlaceName != placeNameToSearch)
                                                        .OrderBy(p => Guid.NewGuid())
                                                        .Select(place => new PlaceRecommendationDto
                                                        {
                                                            PlaceName = preferredLanguage != "en"
                                                                        ? _translationService.TranslateTextResultASync(place.PlaceName, preferredLanguage)
                                                                        : place.PlaceName,
                                                            Image = GetPlaceImageUrl(place),
                                                            Rate = _context.Rating
                                                                        .Where(r => r.PlaceId == place.Id)
                                                                        .Select(r => r.Rate)
                                                                        .FirstOrDefault()
                                                        })
                                                        .Take(3)
                                                        .ToList();

                            // Combine recommended and nearest places, exclude requested place
                            var combinedPlaces = recommendedPlaces.Concat(nearestPlaces)
                                                                .Where(c => c.PlaceName != placeNameToSearch)
                                                                .GroupBy(p => p.PlaceName)
                                                                .Select(g => g.First())
                                                                .ToList();

                            return combinedPlaces;
                        }
                    }

                }
            }
            catch (Exception ex)
            {
              
                throw; // Rethrow the exception to propagate it up
            }

            return null;
        }

        private string GetPlaceImageUrl(Place place)
        {
            if (place?.PlaceMedias == null)
            {
                return null;
            }

            var mediaContent = place.PlaceMedias.FirstOrDefault(m => m.MediaType.ToLower() == "image")?.MediaContent;
            if (mediaContent != null)
            {
                return _blobStorageService.GetBlobUrlmedia(mediaContent);
            }
            else
            {  
                return null;
            }
        }
    }
}

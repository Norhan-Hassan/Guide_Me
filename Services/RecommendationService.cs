using Guide_Me.DTO;
using Guide_Me.Models;
using Microsoft.EntityFrameworkCore;
using static System.Net.Mime.MediaTypeNames;

namespace Guide_Me.Services
{
    public class RecommendationService : IRecommendationService
    {
        private readonly ApplicationDbContext _context;
        private readonly ITouristService _touristService;
        private readonly IPlaceService _placeService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public RecommendationService(ApplicationDbContext context,ITouristService touristService, IPlaceService placeService, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _touristService = touristService;
            _placeService = placeService;
            _httpContextAccessor = httpContextAccessor;
        }

        public List<PlaceRecommendationDto> GetRecommendationByPreferences(string touristName, string cityName)
        {
            string touristID = _touristService.GetUserIdByUsername(touristName);

            if (!string.IsNullOrEmpty(touristID))
            {
                var favPlaces = _context.Favorites
                                        .Where(f => f.TouristID == touristID && f.Place.City.CityName == cityName)
                                        .Select(f => f.Place)
                                        .ToList();

                var histPlaces = _context.histories
                                         .Where(h => h.TouristId == touristID && h.Place.City.CityName == cityName)
                                         .Select(h => h.Place)
                                         .ToList();

                var commonPlaces = favPlaces.Concat(histPlaces).ToList();

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
                        var allPlacesInCity = _context.Places
                                                      .Include(p => p.PlaceMedias) 
                                                      .Where(p => p.City.CityName == cityName && p.Category.ToLower() == mostFrequentCategory)
                                                      .ToList();

                        var recommendedPlaces = allPlacesInCity
                                                 .Where(place => !favPlaces.Contains(place) && !histPlaces.Contains(place))
                                                 .Select(place => {
                                                     var mediaContent = place.PlaceMedias
                                                                             ?.FirstOrDefault(m => m.MediaType.ToLower() == "image")
                                                                             ?.MediaContent;

                                                     
                                                     Console.WriteLine($"Place: {place.PlaceName}, Media Content: {mediaContent}");

                                                     return new PlaceRecommendationDto
                                                     {
                                                         PlaceName = place.PlaceName,
                                                         Image = _placeService.GetMediaUrl(mediaContent),
                                                         Rate = place.Ratings != null && place.Ratings.Any() ? place.Ratings.Average(r => r.Rate) : 0
                                                     };
                                                 })
                                                .Take(3)
                                                .ToList();
                        return recommendedPlaces;
                    }
                }
            }

            return new List<PlaceRecommendationDto>();
        }




    }
}

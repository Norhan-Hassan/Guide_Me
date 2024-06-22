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

        public  List<PlaceRecommendationDto> GetRecommendation(string touristName, string cityName , double Latitude, double Longitude)
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
                var nearestPlaces = _context.Places
                               .Where(p => p.City.CityName == cityName)
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
                                   PlaceName = p.Place.PlaceName,
                                   Image = _placeService.GetMediaUrl(p.Place.PlaceMedias
                                       ?.FirstOrDefault(m => m.MediaType.ToLower() == "image")
                                       ?.MediaContent),
                                   Rate = _context.Rating
                                          .Where(r => r.PlaceId == p.Place.Id)
                                           .Select(r => r.Rate)
                                           .FirstOrDefault()
                               })
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
                                                         Rate = _context.Rating
                                                            .Where(r => r.PlaceId == place.Id) 
                                                            .Select(r => r.Rate)
                                                            .FirstOrDefault()
                                                     };
                                                 })
                                                .Take(3)
                                                .ToList();
                        var combinedPlaces = nearestPlaces.Concat(recommendedPlaces)
                                             .GroupBy(p => p.PlaceName)
                                             .Select(g => g.First()) 
                                             .ToList();

                        return combinedPlaces;
                    }
                }
            }

            return null;
        }


        private static double DegreeToRadian(double degree)
        {
            return degree * Math.PI / 180.0;
        }

    }
}

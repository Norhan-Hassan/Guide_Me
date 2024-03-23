using Guide_Me.DTO;
using Guide_Me.Models;
using System.Linq;
using Guide_Me.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Azure.Core;
using Microsoft.EntityFrameworkCore;


namespace Guide_Me.Services
{
    public class FavoritePlaceService : IFavoritePlaceService
    {
        private readonly ApplicationDbContext _context;
        private readonly ITouristService _ITouristService;
        private readonly IPlaceService _IPlaceService;
        private readonly ILogger<FavoritePlaceService> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public FavoritePlaceService(ApplicationDbContext context, ITouristService ITouristService, IPlaceService IPlaceService, ILogger<FavoritePlaceService> logger, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _ITouristService = ITouristService;
            _IPlaceService = IPlaceService;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        public void MarkFavoritePlace(FavouritePlacesDto request)
        {
            //try
            //{
            var touristId = _ITouristService.GetUserIdByUsername(request.TouristName);
            var placeId = _IPlaceService.GetPlaceIdByPlaceName(request.PlaceName);

            var existingFavorite = _context.Favorites.FirstOrDefault(f => f.PlaceID == placeId && f.TouristID == touristId);

            if (existingFavorite == null)
            {
                // Add new favorite place
                _context.Favorites.Add(new Favorite
                {
                    TouristID = touristId,
                    PlaceID = placeId
                });

            }
            else
            {
                // Remove existing favorite place
                _context.Favorites.Remove(existingFavorite);
            }

            _context.SaveChanges();
        }


        public List<PlaceDto> GetAllFavoritesByTourist(string touristName)
        {
            var touristId = _ITouristService.GetUserIdByUsername(touristName);

            if (string.IsNullOrEmpty(touristId))
            {
                // If the tourist does not exist, return an empty list
                return new List<PlaceDto>();
            }

            var favoritePlaces = _context.Favorites
                .Where(f => f.TouristID == touristId)
                .Select(f => f.PlaceID)
                .ToList();

            var places = _context.Places
                .Where(p => favoritePlaces.Contains(p.Id))
                .Select(p => new PlaceDto
                {
                    Name = p.PlaceName,
                    Category = p.Category,
                    Media = p.PlaceMedias.Select(pm => new PlaceMediaDto
                    {
                        MediaType = pm.MediaType,
                        MediaContent = pm.MediaType.ToLower() == "text" ? pm.MediaContent : GetMediaUrl(pm.MediaContent, _httpContextAccessor.HttpContext)
                    }).ToList()
                })
                .ToList();

            return places;
        }

        private static string GetMediaUrl(string mediaContent, HttpContext httpContext)
        {
            var request = httpContext.Request;
            var baseUrl = $"{request.Scheme}://{request.Host}";
            return $"{baseUrl}/{mediaContent}";
        }
    }
}



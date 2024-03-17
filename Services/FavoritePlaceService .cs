using Guide_Me.DTO;
using Guide_Me.Models;
using System.Linq;
using Guide_Me.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Azure.Core;

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
        //catch (Exception ex)
        //{
        //    // Log the exception
        //    _logger.LogError(ex, "An error occurred while saving changes to the database: {Message}", ex.Message);
        //    throw; // Rethrow the exception to propagate it further if necessary
        //}
        //public List<PlaceDto> GetAllFavoritesByTourist(string touristName) 
        //{
        //        List<PlaceDto> Places = new List<PlaceDto>();
        //    var touristid = _ITouristService.GetUserIdByUsername(touristName);       
        //    var favorites = _context.Favorites;

        //        foreach ( var item  in  favorites)
        //        {
        //            if (item.TouristID == touristid)
        //            {

        //            }

        //        }

        //}

    }
}


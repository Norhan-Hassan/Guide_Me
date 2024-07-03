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
        private readonly IBlobStorageService _blobStorageService;
        private readonly ITranslationService _translationService;

        public FavoritePlaceService(ApplicationDbContext context, 
            ITouristService ITouristService,
            IPlaceService IPlaceService, 
            ILogger<FavoritePlaceService> logger,
            IHttpContextAccessor httpContextAccessor,
            IBlobStorageService blobStorageService,
            ITranslationService translationService)
        {
            _context = context;
            _ITouristService = ITouristService;
            _IPlaceService = IPlaceService;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            _blobStorageService = blobStorageService;
            _translationService= translationService;
        }

        public void MarkFavoritePlace(FavouritePlacesDto request)
        {
            var tourist = _ITouristService.GetTouristByUsername(request.TouristName);
            if (tourist == null)
            {

                throw new ArgumentException("Tourist not found.");
            }

            var targetLanguage = tourist.Language;
            string translatedPlaceName = request.PlaceName;

            // Translate placeName to English if tourist's preferred language is not English
            if (targetLanguage != "en")
            {
                translatedPlaceName = _translationService.TranslateTextResultASync(request.PlaceName, "en");
            }

            var touristId = _ITouristService.GetUserIdByUsername(request.TouristName);
            var placeId = _IPlaceService.GetPlaceIdByPlaceName(translatedPlaceName);

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


        public List<FavoritePlaceDtoreturn> GetAllFavoritesByTourist(string touristName)
        {
            var tourist = _ITouristService.GetTouristByUsername(touristName);
            if (tourist == null)
            {
    
                return null;
            }

            var targetLanguage = tourist.Language;
            var touristId = tourist.Id;

            // Fetch all favorite place IDs for the tourist
            var favoritePlaces = _context.Favorites
                .Where(f => f.TouristID == touristId)
                .Select(f => f.PlaceID)
                .ToList();

            // Fetch details for all favorite places
            var places = _context.Places
                .Where(p => favoritePlaces.Contains(p.Id))
                .Select(p => new FavoritePlaceDtoreturn
                {
                    Name = targetLanguage != "en" ? _translationService.TranslateTextResultASync(p.PlaceName, targetLanguage) : p.PlaceName,
                    Category = p.Category,
                    FavoriteFlag = 1, 
                    Media = p.PlaceMedias
                        .Where(pm => pm.MediaType.ToLower() == "image")
                        .Select(pm => new PlaceMediaDto
                        {
                            MediaType = pm.MediaType,
                            MediaContent = _blobStorageService.GetBlobUrlmedia(pm.MediaContent)
                        }).ToList()
                })
                .ToList();

            return places;
        }


    }
}



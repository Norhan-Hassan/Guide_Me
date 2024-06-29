using Guide_Me.DTO;
using Guide_Me.Migrations;
using Guide_Me.Models;
using Microsoft.EntityFrameworkCore;
using System.Numerics;
using System.Linq;
using System.Web;

namespace Guide_Me.Services
{
    public class PlaceService : IPlaceService
    {
        
            private readonly ApplicationDbContext _context;
            private readonly IHttpContextAccessor _httpContextAccessor;
            private readonly ITranslationService _translationService;

            public PlaceService(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor, ITranslationService translationService)
            {
                _context = context;
                _httpContextAccessor = httpContextAccessor;
                _translationService = translationService;
            }

            public List<PlaceItemDto> GetPlaceItems(string placeName)
        {
            var place = _context.Places.FirstOrDefault(p => p.PlaceName == placeName);
            var placeItemsMap = new List<PlaceItemDto>();

            if (place != null)
            {

                var placeItems = _context.placeItem
                    .Include(p => p.PlaceItemMedias)
                    .Where(pi => pi.placeID == place.Id)
                    .ToList();

                foreach (var placeItem in placeItems)
                {
                    PlaceWithoutMediaDto placeDto = new PlaceWithoutMediaDto
                    {
                        Name = place.PlaceName,
                        Category = place.Category,

                    };

                    var placeItemDto = new PlaceItemDto
                    {
                        ID = placeItem.ID,
                        placeItemName = placeItem.placeItemName,
                        Media = placeItem.PlaceItemMedias != null ?
                         placeItem.PlaceItemMedias.Select(media => new ItemMediaDto
                         {
                             MediaContent = media.MediaContent,
                             MediaType = media.MediaType,
                         })
                         .ToList()
                         : new List<ItemMediaDto>()

                    };

                    placeItemsMap.Add(placeItemDto);
                }
            }

            return placeItemsMap;
        }


        public List<PlaceDto> GetPlaces(string cityName, string touristName)
        {
            var city = _context.Cities.FirstOrDefault(c => c.CityName == cityName);
            if (city == null)
            {
                return null; // or handle appropriately
            }

            var tourist = _context.Tourist.FirstOrDefault(t => t.UserName == touristName);
            if (tourist == null)
            {
                return null; // or handle appropriately
            }

            var preferredLanguage = tourist.Language;

            var favoritePlaceIds = _context.Favorites
                                          .Where(f => f.TouristID == tourist.Id)
                                          .Select(f => f.PlaceID)
                                          .ToList();

            var places = _context.Places
                                .Include(p => p.PlaceMedias)
                                .Where(p => p.CityId == city.Id)
                                .ToList();

            List<PlaceDto> placeDtos = new List<PlaceDto>();

            foreach (var place in places)
            {
                var translatedName = _translationService.TranslateTextAsync(place.PlaceName, preferredLanguage).Result; // Use .Result to get the synchronous result
                var translatedCategory = _translationService.TranslateTextAsync(place.Category, preferredLanguage).Result;

                var placeDto = new PlaceDto
                {
                    Name = translatedName,
                    Category = translatedCategory,
                    Media = place.PlaceMedias?
                           .Where(m => m.MediaType.ToLower() == "image") // Filter only image media types
                           .Select(m => new PlaceMediaDto
                           {
                               MediaType = m.MediaType,
                               MediaContent = GetMediaUrl(m.MediaContent)
                           })
                           .ToList() ?? new List<PlaceMediaDto>(),
                    FavoriteFlag = favoritePlaceIds.Contains(place.Id) ? 1 : 0 // Check if place is a favorite
                };

                placeDtos.Add(placeDto);
            }

            return placeDtos;
        }


        public List<PlaceMediaDto> GetPlaceMedia(string placeName)
        {
            var place = _context.Places.FirstOrDefault(p => p.PlaceName == placeName);
            var placeMediaMap = new List<PlaceMediaDto>();

            if (place != null)
            {
                var placeMedia = _context.placeMedias
                    .Where(pm => pm.PlaceId == place.Id)
                    .ToList();

                foreach (var media in placeMedia)
                {
                    var mediaDto = new PlaceMediaDto();

                    if (media.MediaType == "image" || media.MediaType == "video" || media.MediaType == "audio")
                    {
                        mediaDto.MediaType = media.MediaType;
                        mediaDto.MediaContent = (media.MediaType == "image" || media.MediaType == "video" || media.MediaType == "audio") ? GetMediaUrl(media.MediaContent) : media.MediaContent;
                    }
                    else
                    {
                        mediaDto.MediaType = media.MediaType;
                        mediaDto.MediaContent = media.MediaContent;
                    }

                    placeMediaMap.Add(mediaDto);
                }
            }

            return placeMediaMap;
        }

        public string GetMediaUrl(string mediaContent)
        {

            var request = _httpContextAccessor.HttpContext.Request;
            var baseUrl = $"{request.Scheme}://{request.Host}";
            return $"{baseUrl}/{mediaContent}";
        }

        public PlaceLocationDto GetLocation(string placeName)
        {
            var place = _context.Places.FirstOrDefault(p => p.PlaceName == placeName);
            if (place == null)
            {
                throw new Exception("Place not found");
            }
            else
            {
                var PlaceLocationDto = new PlaceLocationDto();
                PlaceLocationDto.latitude = place.latitude;
                PlaceLocationDto.longitude = place.longitude;

                return PlaceLocationDto;
            }

        }
        public int GetPlaceIdByPlaceName(string Placename)
        {
            var place = _context.Places.FirstOrDefault(p => p.PlaceName == Placename);

            return place != null ? place.Id : 0;
        }

        public SearchPlaceDto SerachPlace(string placeName , string cityName)
        {
            int placeid = GetPlaceIdByPlaceName(placeName);
            if(placeid > 0)
            {
                var place = _context.Places.FirstOrDefault(p => p.PlaceName == placeName && p.City.CityName == cityName);
                var placeMedia = _context.placeMedias.FirstOrDefault(pm => pm.PlaceId == place.Id && pm.MediaType == "image");

                var placeDto = new SearchPlaceDto
                {
                    placeName = placeName,
                    placeImage = placeMedia != null ? GetMediaUrl(placeMedia.MediaContent) : null

                };
                return placeDto;
            }
            return null;
        }

    }
}
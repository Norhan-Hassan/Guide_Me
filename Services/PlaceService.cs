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
            private readonly IBlobStorageService _blobStorageService;

        public PlaceService(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor, ITranslationService translationService, IBlobStorageService blobStorageService)
            {
                _context = context;
                _httpContextAccessor = httpContextAccessor;
                _translationService = translationService;
                _blobStorageService= blobStorageService;
             }

        public List<PlaceItemDto> GetPlaceItems(string placeName, string touristName)
        {
            var tourist = _context.Tourist.FirstOrDefault(t => t.UserName == touristName);
            var preferredLanguage = tourist?.Language ?? "en"; // Default to English if tourist is null

            // Translate the place name to English if the preferred language is not English
            string placeNameToSearch = placeName;
            if (preferredLanguage != "en")
            {
                placeNameToSearch = _translationService.TranslateTextResultASync(placeName, "en");
            }

            var place = _context.Places.FirstOrDefault(p => p.PlaceName == placeNameToSearch);
            var placeItemsMap = new List<PlaceItemDto>();

            if (place != null)
            {
                var placeItems = _context.placeItem
                    .Include(pi => pi.PlaceItemMedias)
                    .Where(pi => pi.ID == place.Id)
                    .ToList();

                foreach (var placeItem in placeItems)
                {
                    // Translate place item name if the preferred language is not English
                    string placeItemNameToUse = placeItem.placeItemName;
                    if (preferredLanguage != "en")
                    {
                        placeItemNameToUse = _translationService.TranslateTextResultASync(placeItem.placeItemName, preferredLanguage);
                    }

                    var placeItemDto = new PlaceItemDto
                    {
                        ID = placeItem.ID,
                        placeItemName = placeItemNameToUse,
                        Media = placeItem.PlaceItemMedias != null ?
                            placeItem.PlaceItemMedias.Select(media =>
                            {
                                var mediaDto = new ItemMediaDto
                                {
                                    MediaType = media.MediaType
                                };

                                // Translate media content if it's not an image, video, or audio
                                if (media.MediaType == "image" || media.MediaType == "video" || media.MediaType == "audio")
                                {
                                    mediaDto.MediaContent = _blobStorageService.GetBlobUrlmedia(media.MediaContent);
                                }
                                else
                                {
                                    mediaDto.MediaContent = media.MediaContent;
                                    if (preferredLanguage != "en")
                                    {
                                        mediaDto.MediaContent = _translationService.TranslateTextResultASync(media.MediaContent, preferredLanguage);
                                    }
                                }

                                return mediaDto;
                            }).ToList()
                            : new List<ItemMediaDto>()
                    };

                    placeItemsMap.Add(placeItemDto);
                }
            }

            return placeItemsMap;
        }



        public List<PlaceDto> GetPlaces(string cityName, string touristName)
        {
            var tourist = _context.Tourist.FirstOrDefault(t => t.UserName == touristName);
        
            var preferredLanguage = tourist.Language;

            // Translate the city name to English if the preferred language is not English
            string cityNameToSearch = cityName;
            if (preferredLanguage != "en")
            {
                cityNameToSearch = _translationService.TranslateTextResultASync(cityName, "en");
            }

            var city = _context.Cities.FirstOrDefault(c => c.CityName.ToLower() == cityNameToSearch.ToLower());
            if (city == null || tourist == null)
            {
                return null;
            }

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
                string placeNameToUse = place.PlaceName;
                string categoryToUse = place.Category;

                if (preferredLanguage != "en")
                {
                    placeNameToUse = _translationService.TranslateTextResultASync(place.PlaceName, preferredLanguage);
                    categoryToUse = _translationService.TranslateTextResultASync(place.Category, preferredLanguage);
                }

                var placeDto = new PlaceDto
                {
                    Name = placeNameToUse,
                    Category = categoryToUse,
                    latitude = place.latitude,
                    longtitude = place.longitude,
                    Media = place.PlaceMedias?
                           .Where(m => m.MediaType.ToLower() == "image") // Filter only image media types
                           .Select(m => new PlaceMediaDto
                           {
                               MediaType = m.MediaType,
                               MediaContent = _blobStorageService.GetBlobUrlmedia(m.MediaContent)
                           })
                           .ToList() ?? new List<PlaceMediaDto>(),
                    FavoriteFlag = favoritePlaceIds.Contains(place.Id) ? 1 : 0
                };

                placeDtos.Add(placeDto);
            }

            return placeDtos;
        }



        public List<PlaceMediaDto> GetPlaceMedia(string placeName, string touristName)
        {
           
            var tourist = _context.Tourist.FirstOrDefault(t => t.UserName == touristName);
            var preferredLanguage = tourist?.Language ?? "en"; // Default to English if tourist is null

            // Translate the place name to English if the preferred language is not English
            string placeNameToSearch = placeName;
            if (preferredLanguage != "en")
            {
                placeNameToSearch = _translationService.TranslateTextResultASync(placeName, "en");
            }

            var place = _context.Places.FirstOrDefault(p => p.PlaceName == placeNameToSearch);
            var placeMedias = new List<PlaceMediaDto>();

            if (place != null)
            {
                var placeMedia = _context.placeMedias
                    .Where(pm => pm.PlaceId == place.Id &&( pm.MediaType == "image" || pm.MediaType=="text" || pm.MediaType=="video"))
                    .ToList();

                foreach (var media in placeMedia)
                {
                    
                    var mediaDto = new PlaceMediaDto
                    {
                        MediaType = media.MediaType
                    };

                    if (media.MediaType == "image" || media.MediaType == "video")// || media.MediaType == "audio")
                    {
                        mediaDto.MediaContent = _blobStorageService.GetBlobUrlmedia(media.MediaContent);
                    }
                   
                    else if(media.MediaType == "text")
                    {
                        mediaDto.MediaContent = media.MediaContent;
                    }

                    // Translate media content if it's text
                    if (preferredLanguage != "en" && mediaDto.MediaType == "text")
                    {
                        mediaDto.MediaContent = _translationService.TranslateTextResultASync(mediaDto.MediaContent, preferredLanguage);
                    }

                    placeMedias.Add(mediaDto);
                }
            }

            return placeMedias;
        }

        public PlaceLocationDto GetLocation(string placeName , string touristName)
        {
            var tourist = _context.Tourist.FirstOrDefault(t => t.UserName == touristName);
            var preferredLanguage = tourist?.Language ?? "en"; // Default to English if tourist is null

            // Translate the place name to English if the preferred language is not English
            string placeNameToSearch = placeName;
            if (preferredLanguage != "en")
            {
                placeNameToSearch = _translationService.TranslateTextResultASync(placeName, "en");
            }
            var place = _context.Places.FirstOrDefault(p => p.PlaceName == placeNameToSearch);
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

        public SearchPlaceDto SerachPlace(string placeName , string cityName, string touristName)
        {
            var tourist = _context.Tourist.FirstOrDefault(t => t.UserName == touristName);
            var preferredLanguage = tourist?.Language ?? "en"; // Default to English if tourist is null

            // Translate the place name to English if the preferred language is not English
            string placeNameToSearch = placeName;
            if (preferredLanguage != "en")
            {
                placeNameToSearch = _translationService.TranslateTextResultASync(placeName, "en");
            }
            string cityNameToSearch = cityName;
            if (preferredLanguage != "en")
            {
                cityNameToSearch = _translationService.TranslateTextResultASync(cityName, "en");
            }

            int placeid = GetPlaceIdByPlaceName(placeNameToSearch);
            if(placeid > 0)
            {
                var place = _context.Places.FirstOrDefault(p => p.PlaceName == placeNameToSearch && p.City.CityName == cityNameToSearch);
                var placeMedia = _context.placeMedias.FirstOrDefault(pm => pm.PlaceId == place.Id && pm.MediaType == "image");

                var placeDto = new SearchPlaceDto
                {
                    placeName = placeNameToSearch,
                    placeImage = placeMedia != null ? _blobStorageService.GetBlobUrlmedia(placeMedia.MediaContent) : null

                };
                return placeDto;
            }
            return null;
        }

        public string GetMediaUrl(string mediaContent)
        {

            var request = _httpContextAccessor.HttpContext.Request;
            var baseUrl = $"{request.Scheme}://{request.Host}";
            return $"{baseUrl}/{mediaContent}";
        }

    }
}
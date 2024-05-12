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
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ApplicationDbContext _context;
        public PlaceService(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
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


        public List<PlaceDto> GetPlaces(string CityName)
        {
            var city = _context.Cities.FirstOrDefault(c => c.CityName == CityName);
            if (city == null)
            {
                return null;
            }

            var places = _context.Places
                         .Include(p => p.PlaceMedias)
                         .Where(p => p.CityId == city.Id)
                         .ToList();

            List<PlaceDto> placeDtos = new List<PlaceDto>();

            foreach (var place in places)
            {
                PlaceDto placeDto = new PlaceDto
                {
                    Name = place.PlaceName,
                    Category = place.Category,
                    Media = place.PlaceMedias != null
                        ? place.PlaceMedias
                            .Where(m => m.MediaType.ToLower() == "image") // Filter only image media types
                            .Select(m => new PlaceMediaDto
                            {
                                MediaType = m.MediaType,
                                MediaContent = GetMediaUrl(m.MediaContent)
                            }).ToList()
                        : new List<PlaceMediaDto>()
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








        private string GetMediaUrl(string mediaContent)
        {

            var request = _httpContextAccessor.HttpContext.Request;
            var baseUrl = $"{request.Scheme}://{request.Host}";
            return $"{baseUrl}/{mediaContent}";
        }


        public async Task PostLocationAsync(string placeName, double latitude, double longitude)
        {
            var place = await _context.Places.FirstOrDefaultAsync(p => p.PlaceName == placeName);
            if (place == null)
            {
                throw new Exception("Place not found");
            }
            else
            {
                place.latitude = latitude;
                place.longitude = longitude;

                await _context.SaveChangesAsync();
            }

        }
        public int GetPlaceIdByPlaceName(string Placename)
        {
            var place = _context.Places.FirstOrDefault(p => p.PlaceName == Placename);

            return place != null ? place.Id : 0;
        }


    }
}
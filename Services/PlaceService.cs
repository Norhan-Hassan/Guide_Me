using Guide_Me.DTO;
using Guide_Me.Models;
using Microsoft.EntityFrameworkCore;

namespace Guide_Me.Services
{
    public class PlaceService: IPlaceService
    {
        private readonly ApplicationDbContext _context;
        public PlaceService(ApplicationDbContext context) {
            _context = context;
        }
       public List<PlaceDto> GetPlaces(string cityName) {
            
            var city = _context.Cities.FirstOrDefault(c => c.CityName == cityName);
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
            ? place.PlaceMedias.Select(m => new PlaceMediaDto
            {
                MediaContent = m.MediaContent
            }).ToList()
            : new List<PlaceMediaDto>()
                };
                placeDtos.Add(placeDto);
            }
            return placeDtos;
        



    }
    }
}

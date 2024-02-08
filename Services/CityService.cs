using Guide_Me.DTO;
using Guide_Me.Models;

namespace Guide_Me.Services
{
    public class CityService : ICityService
    {
        private readonly ApplicationDbContext _context;

        public CityService(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<CityDto> GetAllCities()
        {
           var cities=  _context.Cities.ToList();
            List<CityDto> cityDtos = new List<CityDto>();

            
            foreach (var city in cities)
            {
                CityDto cityDto = new CityDto
                {
                    Name = city.CityName,
                    
                };
                cityDtos.Add(cityDto);
            }

            
            return cityDtos;


        }

    }
}

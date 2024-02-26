using Guide_Me.DTO;
using Guide_Me.Models;

namespace Guide_Me.Services
{
    public class CityService : ICityService
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public CityService(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
           
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public List<CityDto> GetAllCities()
        {
           var cities=  _context.Cities.ToList();
            List<CityDto> cityDtos = new List<CityDto>();

            
            foreach (var city in cities)
            {
                CityDto cityDto = new CityDto
                {
                    Id=city.Id,
                    Name = city.CityName,
                    CityImage = GetMediaUrl(city.CityImage),

                };
                cityDtos.Add(cityDto);
            }

            
            return cityDtos;


        }
        private string GetMediaUrl(string CityImage)
        {

            var request = _httpContextAccessor.HttpContext.Request;
            var baseUrl = $"{request.Scheme}://{request.Host}";
            return $"{baseUrl}/{CityImage}";
        }

    }
}

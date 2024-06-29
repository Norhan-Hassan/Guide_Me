using Guide_Me.DTO;
using Guide_Me.Models;

namespace Guide_Me.Services
{
    public class CityService : ICityService
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IBlobStorageService _blobStorageService;
        public CityService(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor, IBlobStorageService blobStorageService)
        {
           
            _context = context;
            _httpContextAccessor = httpContextAccessor;
             _blobStorageService= blobStorageService;
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
                    //CityImage = GetMediaUrl(city.CityImage),
                    CityImage = GetBlobUrlmedia(city.CityImage),

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

        private string GetBlobUrlmedia(string CityImage)
        {
            // Replace this with actual container and blob name based on your storage structure
            string containerName = "firstcontainer";
            // Assuming CityImage is the blob name or a unique identifier for the blob
            string blobName = CityImage;
            // Call BlobStorageService to get the blob URL
            return _blobStorageService.GetBlobUrl(containerName, blobName);
        }

    }
}

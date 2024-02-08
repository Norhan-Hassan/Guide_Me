using Guide_Me.DTO;
using Guide_Me.Models;

namespace Guide_Me.Services
{
    public interface ICityService
    {
        List<CityDto> GetAllCities();
    }
}

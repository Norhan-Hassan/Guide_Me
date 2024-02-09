using Guide_Me.DTO;
using Guide_Me.Models;

namespace Guide_Me.Services
{
    public interface IPlaceService
    {
        List<PlaceDto> GetPlaces(string cityName);
        Dictionary<PlaceDto, List<PlaceItemDto>> GetPlaceItems(string placeName);
    }
}

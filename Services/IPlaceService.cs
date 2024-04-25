using Guide_Me.Models;
using Guide_Me.DTO;

namespace Guide_Me.Services
{
    public interface IPlaceService
    {
        List<PlaceDto> GetPlaces(string cityName);
        List<PlaceItemDto> GetPlaceItems(string placeName);
        Task PostLocationAsync(string placeName, double latitude, double longitude);
        int GetPlaceIdByPlaceName(string placeName);

        public List<PlaceMediaDto> GetPlaceMedia(string placeName);

    }
}


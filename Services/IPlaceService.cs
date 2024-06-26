using Guide_Me.Models;
using Guide_Me.DTO;

namespace Guide_Me.Services
{
    public interface IPlaceService
    {
        List<PlaceDto> GetPlaces(string cityName, string touristName);
        List<PlaceItemDto> GetPlaceItems(string placeName);
        PlaceLocationDto GetLocation(string placeName);
        int GetPlaceIdByPlaceName(string placeName);
        string GetMediaUrl(string mediaContent);
        List<PlaceMediaDto> GetPlaceMedia(string placeName);

        SearchPlaceDto SerachPlace(string placeName, string cityName);

    }
}


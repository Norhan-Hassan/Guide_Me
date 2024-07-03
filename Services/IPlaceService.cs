using Guide_Me.Models;
using Guide_Me.DTO;

namespace Guide_Me.Services
{
    public interface IPlaceService
    {
        List<PlaceDto> GetPlaces(string cityName, string touristName);
        List<PlaceItemDto> GetPlaceItems(string placeName, string touristName);
        List<PlaceMediaDto> GetPlaceMedia(string placeName, string touristName);
        string GetMediaUrl(string mediaContent);
        PlaceLocationDto GetLocation(string placeName, string touristName);
        int GetPlaceIdByPlaceName(string Placename);
        SearchPlaceDto SerachPlace(string placeName, string cityName, string touristName);
    }
}


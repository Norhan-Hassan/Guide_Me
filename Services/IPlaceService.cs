using Guide_Me.DTO;

namespace Guide_Me.Services
{
    public interface IPlaceService
    {
        List<PlaceDto> GetPlaces(string cityName);
    }
}

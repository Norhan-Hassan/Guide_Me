using Guide_Me.DTO;

namespace Guide_Me.Services
{
    public interface IRecommendationService
    {
         List<PlaceRecommendationDto> GetRecommendation(string touristName, string cityName, double lat, double lon);
    }
}

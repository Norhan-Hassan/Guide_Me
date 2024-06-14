using Guide_Me.DTO;

namespace Guide_Me.Services
{
    public interface IRecommendationService
    {
        List<PlaceRecommendationDto> GetRecommendationByPreferences(string touristName, string cityName);
    }
}

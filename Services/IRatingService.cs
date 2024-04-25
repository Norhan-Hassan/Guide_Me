using Guide_Me.DTO;
namespace Guide_Me.Services
{
    public interface IRatingService
    {
        bool RatePlace(RatePlaceDto ratePlaceDto);
        int GetOverAllRateOfPlace(string placeName);
    }
}

using Guide_Me.DTO;
using Guide_Me.Models;
namespace Guide_Me.Services
{
    public interface IRatingService
    {
        bool RatePlace(RatePlaceDto ratePlaceDto);
        int GetOverAllRateOfPlace(string placeName);
        List<string> GetSuggestionsBasedOnRating(int ratenum);
        bool AddSuggestionChoosen(string suggestion, RatePlaceDto ratingDto);
    }
}

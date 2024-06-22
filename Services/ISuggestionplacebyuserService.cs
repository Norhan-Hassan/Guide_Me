using Guide_Me.Models;

namespace Guide_Me.Services
{
    public interface ISuggestionplacebyuserService
    {
        Task SubmitSuggestion(string placeName, string address, double? latitude, double? longitude, string touristname);
    }
}

using Guide_Me.Models;

namespace Guide_Me.Services
{
    public interface ISuggestionplacebyuserService
    {
        Task SubmitSuggestionAsync(string placeName, string address, string touristname);
    }
}

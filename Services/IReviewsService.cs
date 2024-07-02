using Guide_Me.DTO;
using Guide_Me.Models;

namespace Guide_Me.Services
{
    public interface IReviewsService
    {
        bool AddReviewOnPlace(ReviewPlaceDto reviewPlaceDto);
        List<TouristReviewDto> GetReviewOnPlace(string placeName,string touristName);
    }
}

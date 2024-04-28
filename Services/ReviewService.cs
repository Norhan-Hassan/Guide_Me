using Guide_Me.DTO;
using Guide_Me.Migrations;
using Guide_Me.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Reflection.Metadata.Ecma335;

namespace Guide_Me.Services
{
    public class ReviewsService : IReviewsService
    {
        private readonly IPlaceService _IPlaceService;
        private readonly ITouristService _ITouristService;
        private readonly ApplicationDbContext _context;
        public ReviewsService(ApplicationDbContext context, ITouristService ITouristService, IPlaceService IPlaceService)
        {
            _context = context;
            _ITouristService = ITouristService;
            _IPlaceService = IPlaceService;
        }

        public bool AddReviewOnPlace(ReviewPlaceDto reviewPlaceDto)
        {
            var placeId = _IPlaceService.GetPlaceIdByPlaceName(reviewPlaceDto.placeName);
            var touristId = _ITouristService.GetUserIdByUsername(reviewPlaceDto.touristName);

            if (placeId > 0 && touristId != null)
            {

                _context.Reviews.Add(new Review
                {
                    TouristId = touristId,
                    placeId = placeId,
                    Comment = reviewPlaceDto.comment,
                });
                _context.SaveChanges();
                return true;
            }
            else
            {
                return false;
            }
        }

        public List<TouristReviewDto> GetReviewOnPlace(string placeName)
        {
            int placeID = _IPlaceService.GetPlaceIdByPlaceName(placeName);
            List<Review> reviews = new List<Review>();
            List<TouristReviewDto> result = new List<TouristReviewDto>();
            if (placeID > 0)
            {
                foreach (var review in _context.Reviews)
                {
                    reviews.Add(review);
                }
                foreach (var item in reviews)
                {
                    if(item.placeId==placeID)
                    {
                        result.Add(new TouristReviewDto
                        {
                            comment = item.Comment,
                            touristName = _ITouristService.GetUserNameByUserId(item.TouristId),
                        });

                    }
                }
            }
            if (result.Count > 0)
                return result;
            else
                return null;
        }

    }
}

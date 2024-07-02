using Guide_Me.DTO;
using Guide_Me.Models;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Guide_Me.Services
{
    public class ReviewsService : IReviewsService
    {
        private readonly IPlaceService _placeService;
        private readonly ITouristService _touristService;
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ITranslationService _translationService;

        public ReviewsService(ApplicationDbContext context, ITouristService touristService,
                              IPlaceService placeService, IHttpContextAccessor httpContextAccessor,
                              ITranslationService translationService)
        {
            _context = context;
            _touristService = touristService;
            _placeService = placeService;
            _httpContextAccessor = httpContextAccessor;
            _translationService = translationService;
        }

        public bool AddReviewOnPlace(ReviewPlaceDto reviewPlaceDto)
        {
            var placeId = _placeService.GetPlaceIdByPlaceName(reviewPlaceDto.placeName);
            var touristId = _touristService.GetUserIdByUsername(reviewPlaceDto.touristName);

            if (placeId > 0 && touristId != null)
            {
                var tourist = _touristService.GetTouristByUsername(reviewPlaceDto.touristName);
                var targetLanguage = tourist.Language;
                string translatedComment = reviewPlaceDto.comment;

                if (targetLanguage != "en")
                {
                    translatedComment = _translationService.TranslateTextResultASync(reviewPlaceDto.comment, "en");
                }

                _context.Reviews.Add(new Review
                {
                    TouristId = touristId,
                    placeId = placeId,
                    Comment = translatedComment,
                });
                _context.SaveChanges();
                return true;
            }
            else
            {
                return false;
            }
        }

        public List<TouristReviewDto> GetReviewOnPlace(string placeName, string touristName)
        {
            int placeID = _placeService.GetPlaceIdByPlaceName(placeName);
            if (placeID <= 0)
            {
                return null;
            }

            var tourist = _touristService.GetTouristByUsername(touristName);
            if (tourist == null)
            {
                return null;
            }

            var targetLanguage = tourist.Language;

            // Fetch all reviews for the place in a single query and then process them
            var reviews = _context.Reviews.Where(r => r.placeId == placeID).ToList();

            List<TouristReviewDto> result = new List<TouristReviewDto>();

            foreach (var review in reviews)
            {
                string commentToUse = review.Comment;

                // Translate comment to the tourist's preferred language if not already in that language
                if (targetLanguage != "en")
                {
                    commentToUse = _translationService.TranslateTextResultASync(review.Comment, targetLanguage);
                }

                result.Add(new TouristReviewDto
                {
                    comment = commentToUse,
                    touristName = _touristService.GetUserNameByUserId(review.TouristId),
                    PhotoUrl = string.IsNullOrEmpty(_touristService.GetUserPhotoByUserId(review.TouristId))
                                ? null
                                : $"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}/uploads/photos/{Path.GetFileName(_touristService.GetUserPhotoByUserId(review.TouristId))}"
                });
            }

            return result.Count > 0 ? result : null;
        }
    }
}

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

        public  bool AddReviewOnPlace(ReviewPlaceDto reviewPlaceDto)
        {

            var tourist =  _touristService.GetTouristByUsername(reviewPlaceDto.touristName);
            if (tourist == null)
            {
                return false;
            }

            var targetLanguage = tourist.Language;
            string translatedPlaceName = reviewPlaceDto.placeName;

            if (targetLanguage != "en")
            {
                translatedPlaceName =  _translationService.TranslateTextResultASync(reviewPlaceDto.placeName, "en");
            }

            // Get placeId using translated placeName
            var placeId = _placeService.GetPlaceIdByPlaceName(translatedPlaceName);
            if (placeId <= 0)
            {
                return false;
            }

            _context.Reviews.Add(new Review
            {
                TouristId = tourist.Id,
                placeId = placeId,
                Comment= reviewPlaceDto.comment
            });

             _context.SaveChangesAsync();
            return true;
        }

        public List<TouristReviewDto> GetReviewOnPlace(string placeName, string touristName)
        {
            var tourist =  _touristService.GetTouristByUsername(touristName);
            if (tourist == null)
            {
                return null;
            }

            var targetLanguage = tourist.Language;
            string translatedPlaceName = placeName;

            if (targetLanguage != "en")
            {
                translatedPlaceName =  _translationService.TranslateTextResultASync(placeName, "en");
            }

            // Get placeId using translated placeName
            int placeId = _placeService.GetPlaceIdByPlaceName(translatedPlaceName);
            if (placeId <= 0)
            {
                return null;
            }

            // Fetch all reviews for the place
            var reviews = _context.Reviews.Where(r => r.placeId == placeId).ToList();

            List<TouristReviewDto> result = new List<TouristReviewDto>();

            foreach (var review in reviews)
            {
               
                // Construct PhotoUrl if available
                var userPhotoUrl = _touristService.GetUserPhotoByUserId(review.TouristId);
                string photoUrl = null;
                if (!string.IsNullOrEmpty(userPhotoUrl))
                {
                    photoUrl = $"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}/uploads/photos/{Path.GetFileName(userPhotoUrl)}";
                }

                result.Add(new TouristReviewDto
                {
                    comment = review.Comment,
                    touristName = touristName, 
                    PhotoUrl = photoUrl
                });
            }

            return result.Count > 0 ? result : null;
        }
    }
}

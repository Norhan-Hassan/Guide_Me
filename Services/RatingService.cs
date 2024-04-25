using Guide_Me.Services;
using Guide_Me.DTO;
using Guide_Me.Models;
using Azure.Core;
using Guide_Me.Migrations;
namespace Guide_Me.Services
{
    public class RatingService : IRatingService
    {
        private readonly IPlaceService _IPlaceService;
        private readonly ITouristService _ITouristService;
        private readonly ApplicationDbContext _context;
        public RatingService(ApplicationDbContext context, ITouristService ITouristService, IPlaceService IPlaceService)
        {
            _context = context;
            _ITouristService = ITouristService;
            _IPlaceService = IPlaceService;
        }


        //public bool RatePlace(RatePlaceDto ratePlaceDto)
        //{
        //    int placeId = _IPlaceService.GetPlaceIdByPlaceName(ratePlaceDto.placeName);
        //    var TouristID = _ITouristService.GetUserIdByUsername(ratePlaceDto.touristName);
        //    //to check if this tourist or place found in database or not
        //    bool found = false;
        //    if (placeId > 0 && TouristID != null)
        //    {
        //        // Add new rating to this place
        //        _context.Rating.Add(new Rating
        //        {
        //            Rate = ratePlaceDto.ratingNum,
        //            PlaceId = placeId,
        //            TouristId = TouristID,
        //        });
        //        found = true;
        //        _context.SaveChanges();
        //    }
        //    else
        //    {
        //        found = false;
        //    }
        //    return found;
        //}

        public bool RatePlace(RatePlaceDto ratePlaceDto)
        {
            int placeId = _IPlaceService.GetPlaceIdByPlaceName(ratePlaceDto.placeName);
            var touristId = _ITouristService.GetUserIdByUsername(ratePlaceDto.touristName);

            if (placeId <= 0 || touristId == null)
            {
                return false;
            }

            // Check if the tourist has already rated this place or not
            var existingRating = _context.Rating
                               .FirstOrDefault(r => r.PlaceId == placeId && r.TouristId == touristId);

            if (existingRating != null)
            {
                existingRating.Rate = ratePlaceDto.ratingNum;
            }
            else
            {
                // Add new rating to this place
                _context.Rating.Add(new Rating
                {
                    Rate = ratePlaceDto.ratingNum,
                    PlaceId = placeId,
                    TouristId = touristId
                });
            }
            _context.SaveChanges();

            return true;
        }

        public int GetOverAllRateOfPlace(string placeName)
        {
            int placeID = _IPlaceService.GetPlaceIdByPlaceName(placeName);
            List<Rating> rating = _context.Rating.Where(p => p.PlaceId==placeID).ToList();
            int sumOfRating = 0, overallRating = 0;
             

            foreach (Rating rate in rating)
            {
                sumOfRating += rate.Rate;
            }

            if (rating.Count > 0)
            {
                overallRating = sumOfRating / rating.Count;
            }

            return overallRating;
        }


    }
}

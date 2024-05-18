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
        public Dictionary<int, List<string>> suggestions = new Dictionary<int, List<string>>()
        {
            {1, ["Poor Service Content","Cleanliness Concerns","OverCrowding","High Prices","Ineffective Communication","Lack Of Culture Sensitivity"] },
            {2, ["Poor Service Content","Cleanliness Concerns","OverCrowding","High Prices","Effective Safety","Ease Of Access"] },
            {3, ["Poor Service Content","Cleanliness Concerns","OverCrowding","High Prices","Effective Safety","Ease Of Access"] },
            {4, ["Attractive Environment","Engaging Activity","Ease Of Access","Cleanliness Concerns","OverCrowding","High Prices"] },
            {5, ["Attractive Environment","Engaging Activity","Ease Of Access","Effective Safety","Culture Signficance","Sweet soul people"] }
        };
        
        public RatingService(ApplicationDbContext context, ITouristService ITouristService, IPlaceService IPlaceService)
        {
            _context = context;
            _ITouristService = ITouristService;
            _IPlaceService = IPlaceService;

        }

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
            List<Rating> rating = _context.Rating.Where(p => p.PlaceId == placeID).ToList();
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
        public List<string> GetSuggestionsBasedOnRating(int ratenum)
        {
            foreach (var suggest in suggestions)
            {
                if(suggest.Key==ratenum)
                {
                    return suggest.Value;
                }
            }
            return ["Not Found Suggestion For that Rate number"];
        }
        public int GetLatestRateOfToursit(string TouristName , string PlaceName)
        {
            string touristID = _ITouristService.GetUserIdByUsername(TouristName);
            int placeID = _IPlaceService.GetPlaceIdByPlaceName(PlaceName);
            if (placeID > 0 && touristID != null)
            {
                Rating rate = _context.Rating
                    .Where(r => r.TouristId == touristID && r.PlaceId == placeID)
                    .FirstOrDefault();

                if(rate != null)
                    return rate.Rate;
                else
                    return 0;
            }
            return -1;

        }

        public bool AddSuggestionChoosen(RatePlaceWithSuggDto ratingDto)
        {
            string touristID = _ITouristService.GetUserIdByUsername(ratingDto.touristName);
            int placeID = _IPlaceService.GetPlaceIdByPlaceName(ratingDto.placeName);

            if (placeID > 0 && touristID != null)
            {
                Rating rate = _context.Rating
                    .Where(r => r.TouristId == touristID && r.PlaceId == placeID)
                    .FirstOrDefault();

                if (rate != null && rate.Rate == ratingDto.ratingNum)
                {
                    
                    var existingSuggestions = _context.RatingSuggestions
                        .Where(rs => rs.rateID == rate.RatingID)
                        .ToList();

                    _context.RatingSuggestions.RemoveRange(existingSuggestions);

                    foreach (var suggest in ratingDto.suggestion)
                    {
                        _context.RatingSuggestions.Add(new RatingSuggestion
                        {
                            Discription = suggest,
                            rateID = rate.RatingID,
                        });
                    }

                    _context.SaveChanges();
                    return true;
                }
            }

            return false;
        }

    }
}

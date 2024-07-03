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
        private readonly ITranslationService _translationService;
        public Dictionary<int, List<string>> suggestions = new Dictionary<int, List<string>>()
        {
            {1, ["Poor Service Content","Cleanliness Concerns","OverCrowding","High Prices","Ineffective Communication","Lack Of Culture Sensitivity"] },
            {2, ["Poor Service Content","Cleanliness Concerns","OverCrowding","High Prices","Effective Safety","Ease Of Access"] },
            {3, ["Poor Service Content","Cleanliness Concerns","OverCrowding","High Prices","Effective Safety","Ease Of Access"] },
            {4, ["Attractive Environment","Engaging Activity","Ease Of Access","Cleanliness Concerns","OverCrowding","High Prices"] },
            {5, ["Attractive Environment","Engaging Activity","Ease Of Access","Effective Safety","Culture Signficance","Sweet soul people"] }
        };
        
        public RatingService(ApplicationDbContext context,
            ITouristService ITouristService,
            IPlaceService IPlaceService,
            ITranslationService translationService)
        {
            _context = context;
            _ITouristService = ITouristService;
            _IPlaceService = IPlaceService;
            _translationService = translationService;

        }

        public bool RatePlace(RatePlaceDto ratePlaceDto)
        {
            // Retrieve the tourist's preferred language
            var tourist = _context.Tourist.FirstOrDefault(t => t.UserName == ratePlaceDto.touristName);
            var preferredLanguage = tourist?.Language ?? "en"; // Default to English if tourist is null

            // Translate the place name to English for processing if the preferred language is not English
            string placeNameToSearch = ratePlaceDto.placeName;
            if (preferredLanguage != "en")
            {
                placeNameToSearch = _translationService.TranslateTextResultASync(ratePlaceDto.placeName, "en");
            }

            int placeId = _IPlaceService.GetPlaceIdByPlaceName(placeNameToSearch);
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


        public int GetOverAllRateOfPlace(string placeName, string touristName)
        {
            // Retrieve the tourist's preferred language
            var tourist = _context.Tourist.FirstOrDefault(t => t.UserName == touristName);
            var preferredLanguage = tourist?.Language ?? "en"; // Default to English if tourist is null

            // Translate the place name to English for processing if the preferred language is not English
            string placeNameToSearch = placeName;
            if (preferredLanguage != "en")
            {
                placeNameToSearch = _translationService.TranslateTextResultASync(placeName, "en");
            }

            // Get the place ID using the translated place name
            int placeID = _IPlaceService.GetPlaceIdByPlaceName(placeNameToSearch);
            if (placeID <= 0)
            {
                return 0; // Return 0 if place not found
            }

            // Retrieve ratings for the place
            List<Rating> ratings = _context.Rating.Where(p => p.PlaceId == placeID).ToList();
            int sumOfRating = 0, overallRating = 0;

            // Calculate the sum of ratings
            foreach (Rating rate in ratings)
            {
                sumOfRating += rate.Rate;
            }

            // Calculate the overall rating
            if (ratings.Count > 0)
            {
                overallRating = sumOfRating / ratings.Count;
            }

            return overallRating;
        }

        public List<string> GetSuggestionsBasedOnRating(int ratenum, string touristName)
        {
            // Retrieve the tourist's preferred language
            var tourist = _context.Tourist.FirstOrDefault(t => t.UserName == touristName);
            var preferredLanguage = tourist?.Language ?? "en"; // Default to English if tourist is null

            // Initialize a list to store translated suggestions
            List<string> translatedSuggestions = new List<string>();

            // Check if the rating number exists in the suggestions dictionary
            if (suggestions.TryGetValue(ratenum, out List<string> suggestionList))
            {
                foreach (var suggestion in suggestionList)
                {
                    // Translate each suggestion based on the tourist's preferred language
                    string translatedSuggestion = suggestion;
                    if (preferredLanguage != "en")
                    {
                        translatedSuggestion = _translationService.TranslateTextResultASync(suggestion, preferredLanguage);
                    }
                    translatedSuggestions.Add(translatedSuggestion);
                }
                return translatedSuggestions;
            }

            // If the rating number does not exist in the dictionary, return a default message
            string defaultMessage = "Not Found Suggestion For that Rate number";
            if (preferredLanguage != "en")
            {
                defaultMessage = _translationService.TranslateTextResultASync(defaultMessage, preferredLanguage);
            }
            return new List<string> { defaultMessage };
        }

        public int GetLatestRateOfToursit(string TouristName , string PlaceName)
        {

            var tourist = _context.Tourist.FirstOrDefault(t => t.UserName == TouristName);

            var preferredLanguage = tourist.Language;

            // Translate the place name to English if the preferred language is not English
            string placeNameToSearch = PlaceName;
            if (preferredLanguage != "en")
            {
                placeNameToSearch = _translationService.TranslateTextResultASync(PlaceName, "en");
            }

            string touristID = _ITouristService.GetUserIdByUsername(TouristName);
            int placeID = _IPlaceService.GetPlaceIdByPlaceName(placeNameToSearch);

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
            // Retrieve the tourist's details
            var tourist = _context.Tourist.FirstOrDefault(t => t.UserName == ratingDto.touristName);
            if (tourist == null)
            {
                return false; // Tourist not found
            }

            var preferredLanguage = tourist.Language;

            // Translate the place name to English if the preferred language is not English
            string placeNameToSearch = ratingDto.placeName;
            if (preferredLanguage != "en")
            {
                placeNameToSearch = _translationService.TranslateTextResultASync(ratingDto.placeName, "en");
            }
            string touristID = _ITouristService.GetUserIdByUsername(ratingDto.touristName);
            int placeID = _IPlaceService.GetPlaceIdByPlaceName(placeNameToSearch);

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

using Guide_Me.DTO;
using Guide_Me.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace Guide_Me.Services
{
    public class SuggestionplacebyuserService : ISuggestionplacebyuserService
    {
        private readonly ApplicationDbContext _context;
        private readonly ITouristService _ITouristService;
        private readonly ITranslationService _translationService;

        public SuggestionplacebyuserService(ApplicationDbContext context,
            ITouristService ITouristService,
            ITranslationService translationService)
        {
            _context = context;
            _ITouristService = ITouristService;
            _translationService = translationService;
        }

        public async Task SubmitSuggestion(string placeName, string? address, double? latitude, double? longitude, string touristName)
        {
            // Ensure that either address or location (latitude and longitude) is provided
            if (string.IsNullOrEmpty(address) && (!latitude.HasValue || !longitude.HasValue))
            {
                throw new ArgumentException("Either address or location (latitude and longitude) must be provided.");
            }

            var touristId = _ITouristService.GetUserIdByUsername(touristName);
            if (string.IsNullOrEmpty(touristId))
            {
                throw new ArgumentException("Tourist not found", nameof(touristName));
            }

            // Translate placeName and address to English if tourist's preferred language is not English
            var tourist = _context.Tourist.FirstOrDefault(t => t.UserName == touristName);
            var preferredLanguage = tourist?.Language ?? "en"; // Default to English if tourist is null

            string translatedPlaceName = placeName;
            string translatedAddress = address;

            if (preferredLanguage != "en")
            {
                translatedPlaceName = _translationService.TranslateTextResultASync(placeName, "en");
                translatedAddress =  _translationService.TranslateTextResultASync(address, "en");
            }

            // Check if the place already exists
            var existingPlace = await _context.Places.FirstOrDefaultAsync(p => p.PlaceName == translatedPlaceName);
            if (existingPlace != null)
            {
                throw new ArgumentException("Place already exists", nameof(placeName));
            }

            var suggestionplace = new Suggestionplacebyuser
            {
                PlaceName = translatedPlaceName,
                Address = translatedAddress,
                Latitude = latitude,
                Longitude = longitude,
                TouristId = touristId
            };

            _context.Suggestionplacebyusers.Add(suggestionplace);
            await _context.SaveChangesAsync();
        }

    }
}

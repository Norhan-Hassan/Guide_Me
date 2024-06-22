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

        public SuggestionplacebyuserService(ApplicationDbContext context, ITouristService ITouristService)
        {
            _context = context;
            _ITouristService = ITouristService;
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

            var existingPlace = await _context.Places.FirstOrDefaultAsync(p => p.PlaceName == placeName);
            if (existingPlace != null)
            {
                throw new ArgumentException("Place already exists", nameof(placeName));
            }

            var suggestionplace = new Suggestionplacebyuser
            {
                PlaceName = placeName,
                Address = address,
                Latitude = latitude,
                Longitude = longitude,
                TouristId = touristId
            };

            _context.Suggestionplacebyusers.Add(suggestionplace);
            await _context.SaveChangesAsync();
        }
    }
}

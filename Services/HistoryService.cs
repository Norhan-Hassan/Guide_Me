using Guide_Me.Models;
using Guide_Me.DTO;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Guide_Me.Services
{
    public class HistoryService : IHistoryService
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ITouristService _ITouristService;
        private readonly IPlaceService _IPlaceService;
        private readonly ITranslationService _translationService;
        private readonly IBlobStorageService _blobStorageService;
        public HistoryService(ApplicationDbContext context,
            IHttpContextAccessor httpContextAccessor,
            ITouristService ITouristService, 
            IPlaceService IPlaceService,
            ITranslationService translationsService,
            IBlobStorageService blobStorageService)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _ITouristService = ITouristService;
            _IPlaceService = IPlaceService;
            _translationService = translationsService;
            _blobStorageService = blobStorageService;
        }

        public void UpdatePlaceHistory(string placeName, string touristName, DateTime date)
        {
            var touristId = _ITouristService.GetUserIdByUsername(touristName);
            if (touristId == null)
            {
                throw new ArgumentException("Tourist not found", nameof(touristName));
            }

            var tourist = _context.Tourist.FirstOrDefault(t => t.UserName == touristName);
            var preferredLanguage = tourist?.Language ?? "en"; 

            // Translate the place name to English if the preferred language is not English
            string placeNameToSearch = placeName;
            if (preferredLanguage != "en")
            {
                placeNameToSearch = _translationService.TranslateTextResultASync(placeName, "en");
            }

            var placeId = _IPlaceService.GetPlaceIdByPlaceName(placeNameToSearch);
            if (placeId == 0)
            {
                throw new ArgumentException("Place not found", nameof(placeName));
            }

            var newHistoryEntry = new History
            {
                PlaceId = placeId,
                TouristId = touristId,
                Date = date
            };

            _context.histories.Add(newHistoryEntry);
            _context.SaveChanges();
        }

        public List<TouristHistoryDto> GetTouristHistory(string touristName)
        {
            var touristId = _ITouristService.GetUserIdByUsername(touristName);

            if (string.IsNullOrEmpty(touristId))
            {
                return new List<TouristHistoryDto>();
            }

            // Retrieve tourist's preferred language
            var tourist = _context.Tourist.FirstOrDefault(t => t.UserName == touristName);
            var preferredLanguage = tourist?.Language ?? "en";

            var userHistory = (from h in _context.histories
                               where h.TouristId == touristId
                               join p in _context.Places on h.PlaceId equals p.Id
                               let imageMedia = _context.placeMedias
                                                .Where(pm => pm.PlaceId == p.Id && pm.MediaType.ToLower() == "image")
                                                .FirstOrDefault()
                               where imageMedia != null
                               select new TouristHistoryDto
                               {
                                   Place = new PlacehistoryDto
                                   {
                                       Name = preferredLanguage == "en" ? p.PlaceName : _translationService.TranslateTextResultASync(p.PlaceName, preferredLanguage),
                                       Category = preferredLanguage == "en" ? p.Category : _translationService.TranslateTextResultASync(p.Category, preferredLanguage),
                                       Media = new List<PlaceMediaDto>
                                       {
                                           new PlaceMediaDto
                                           {
                                               MediaType = imageMedia.MediaType,
                                               MediaContent = _blobStorageService.GetBlobUrlmedia(imageMedia.MediaContent)
                                           }
                                       }
                                   },
                                   Date = h.Date
                               }).ToList();

            return userHistory;
        }









        public void UpdatePlaceHistory(int placeId, string Touristname, DateTime date)
        {
            throw new NotImplementedException();
        }
    }
}

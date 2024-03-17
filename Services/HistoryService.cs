using Guide_Me.DTO;
using Guide_Me.Models;
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


        public HistoryService(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor, ITouristService ITouristService)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _ITouristService = ITouristService;
        }

        public void UpdatePlaceHistory(int placeId, string Touristname, DateTime date)
        {
            var touristId = _ITouristService.GetUserIdByUsername(Touristname);
            var newHistoryEntry = new History
            {
                PlaceId = placeId,
                TouristId = touristId,
                Date = date
            };

            _context.histories.Add(newHistoryEntry);
            _context.SaveChanges();
        }

        public List<TouristHistoryDto> GetTouristHistory(string Touristname)
        {
            var touristId = _ITouristService.GetUserIdByUsername(Touristname);

            if (string.IsNullOrEmpty(touristId))
            {

                return new List<TouristHistoryDto>();
            }

            var userHistory = (from h in _context.histories
                               join p in _context.Places on h.PlaceId equals p.Id
                               join pm in _context.placeMedias on p.Id equals pm.PlaceId
                               where h.TouristId == touristId
                               select new TouristHistoryDto
                               {
                                   Place = new PlaceDto
                                   {
                                       Name = p.PlaceName,
                                       Category = p.Category,
                                       Media = pm != null ? new List<PlaceMediaDto>
                               {
                                   new PlaceMediaDto
                                   {
                                       MediaType = pm.MediaType,
                                       MediaContent = pm.MediaType.ToLower() == "text" ? pm.MediaContent : GetMediaUrl(pm.MediaContent, _httpContextAccessor.HttpContext)
                                   }
                               } : new List<PlaceMediaDto>()
                                   },
                                   Date = h.Date
                               }).ToList();

            return userHistory;
        }

        private static string GetMediaUrl(string mediaContent, HttpContext httpContext)
        {
            var request = httpContext.Request;
            var baseUrl = $"{request.Scheme}://{request.Host}";
            return $"{baseUrl}/{mediaContent}";
        }


    }
}

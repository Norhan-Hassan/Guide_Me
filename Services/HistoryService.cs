using Guide_Me.Models;
using System;

namespace Guide_Me.Services
{
    public class HistoryService : IHistoryService
    {
        private readonly ApplicationDbContext _context;

        public HistoryService(ApplicationDbContext context)
        {
            _context = context;
        }

        public void UpdatePlaceHistory(int placeId, string touristId, DateTime date)
        {
            var newHistoryEntry = new History
            {
                PlaceId = placeId,
                TouristId = touristId,
                Date = date
            };

            _context.histories.Add(newHistoryEntry);
            _context.SaveChanges();
        }
    }
}

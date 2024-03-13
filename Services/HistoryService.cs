//using Guide_Me.Models;
//using Microsoft.AspNetCore.Http.HttpResults;

//namespace Guide_Me.Services
//{
//    public class HistoryService:IHistoryService
//    {
//        private readonly ApplicationDbContext _context;

//        public HistoryService(ApplicationDbContext context)
//        {
//            _context = context;
//        }
//        public void UpdatePlaceHistory(int placeId, int touristId,  DateTime date)
//        {
           
//            var newHistoryEntry = new History
//            {
//                PlaceId = placeId,
//                TouristId = touristId,
//                Date = date
//            };

//            _context.Histories.Add(newHistoryEntry);
//            _context.SaveChanges();
//        }
//        }
//    }



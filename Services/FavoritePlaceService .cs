using Guide_Me.DTO;
using Guide_Me.Models;
using System.Linq;
using Guide_Me.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace Guide_Me.Services
{
    public class FavoritePlaceService : IFavoritePlaceService
    {
        private readonly ApplicationDbContext _context;

        public FavoritePlaceService(ApplicationDbContext context)
        {
            _context = context;
        }

        public string MarkFavoritePlace(FavouritePlacesDto request)
        {
            var tourist = _context.Users.FirstOrDefault(t => t.Id == request.TouristId);

            if (tourist == null)
            {
                return ("Tourist Not Found ");

            }
            //var existingFavorite = _context.Pr
            //          .FirstOrDefault(fp => fp.PlaceId == request.PlaceId && fp.TouristId == request.TouristId);

            //            if (existingFavorite == null)
            //            {
            //                tourist.FavoritePlaces.Add(new FavoritePlace
            //                {
            //                    PlaceId = request.PlaceId
            //                });

                        return (" added successfully");
        }

       
    }
}
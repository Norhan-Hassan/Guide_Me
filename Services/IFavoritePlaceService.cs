using Guide_Me.DTO;
using Guide_Me.Models;

namespace Guide_Me.Services
{
    public interface IFavoritePlaceService
    {
        void MarkFavoritePlace(FavouritePlacesDto request);

        //List<PlaceDto> GetAllFavoritesByTourist(string touristName);
    }
}
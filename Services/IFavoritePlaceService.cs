using Guide_Me.DTO;
using Guide_Me.Models;

namespace Guide_Me.Services
{
    public interface IFavoritePlaceService
    {
        string MarkFavoritePlace( FavouritePlacesDto request);
    }
}

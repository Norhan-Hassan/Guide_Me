using Guide_Me.DTO;

namespace Guide_Me.Services
{
    public interface IHistoryService
    {
        void UpdatePlaceHistory(int placeId, string Touristname, DateTime date);
        List<TouristHistoryDto> GetTouristHistory(string Touristname);

    }
}
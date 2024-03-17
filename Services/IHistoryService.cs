using Guide_Me.DTO;

namespace Guide_Me.Services
{
    public interface IHistoryService
    {
        void UpdatePlaceHistory(string placename, string Touristname, DateTime date);
        List<TouristHistoryDto> GetTouristHistory(string Touristname);

    }
}
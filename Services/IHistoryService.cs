namespace Guide_Me.Services
{
    public interface IHistoryService
    {
        void UpdatePlaceHistory(int placeId, string touristId,  DateTime date);

    }
}

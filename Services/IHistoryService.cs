namespace Guide_Me.Services
{
    public interface IHistoryService
    {
        void UpdatePlaceHistory(int placeId, int touristId,  DateTime date);

    }
}

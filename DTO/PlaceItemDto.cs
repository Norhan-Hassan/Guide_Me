namespace Guide_Me.DTO
{
    public class PlaceItemDto
    {
        public int ID { get; set; }
        public string placeItemName { get; set; }
        public List<ItemMediaDto> Media { get; set; }
    }
}

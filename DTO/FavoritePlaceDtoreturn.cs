namespace Guide_Me.DTO
{
    public class FavoritePlaceDtoreturn
    {
        public string Name { get; set; }
        public string Category { get; set; }
       
        public int FavoriteFlag { get; set; }
        public List<PlaceMediaDto> Media { get; set; }
    }
}

namespace Guide_Me.Models
{
    public class SuggestedPlaceByUser
    {
        //primary key
        public int SuggID { get; set; }
        // Foreign Key
        public int TouristID { get; set; }
        public string placeName { get; set; }
        public string  Address { get; set; }
    }
}

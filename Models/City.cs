namespace Guide_Me.Models
{
    public class City
    {
        public int  Id { get; set; }

        public string CityName { get; set; }

        public  virtual ICollection<Place> Places { get; set; } 
    }
}

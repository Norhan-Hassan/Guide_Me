namespace Guide_Me.Models
{
    public class City
    {
        public int  Id { get; set; }

        public string CityName { get; set; }


        public ICollection<Place> Places { get; set; } = new List<Place>();
    }
}

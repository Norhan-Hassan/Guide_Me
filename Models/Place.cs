namespace Guide_Me.Models
{
    public class Place
    {
        public int Id { get; set; }

        public string PlaceName { get; set; }

        public string Category { get; set; }

        public int  CityId { get; set; }

        public virtual City City { get; set; }

    }
}

namespace Guide_Me.Models
{
    public class PlaceMedia
    {
        public int Id { get; set; }
        public string MediaType { get; set; }
        public string MediaContent { get; set; }

        public int PlaceId { get; set; }

        public virtual ICollection<PlaceMedia> PlaceMedias { get; set; }
    }
}

using System.ComponentModel.DataAnnotations.Schema;

namespace Guide_Me.Models
{
    public class Rating
    {
        public int RatingID { get; set; }
        public int Rate { get; set; }

        [ForeignKey("Place")]
        public int PlaceId { get; set; }

        [ForeignKey("Tourist")]
        public string TouristId { get; set; }
        public virtual Tourist Tourist { get; set; }
        public virtual Place Place { get; set; }
    }
}

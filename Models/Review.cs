using System.ComponentModel.DataAnnotations.Schema;

namespace Guide_Me.Models
{
    public class Review
    {
        public int ReviewID { get; set; }
        public string  Comment { get; set; }

        [ForeignKey("place")]
        public int placeId { get; set; }
        Place place { get; set; }
        [ForeignKey("Tourist")]
        public string TouristId { get; set; }
        public virtual Tourist Tourist { get; set; }
    }
}

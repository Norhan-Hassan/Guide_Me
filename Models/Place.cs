using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Guide_Me.Models
{

    public class Place
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string PlaceName { get; set; }
        [Required]
        public string Category { get; set; }

        [ForeignKey("City")]
        public int CityId { get; set; }
        [Required]
        public double longitude { get; set; }
        [Required]
        public double latitude { get; set; }
        public virtual City City { get; set; }
        public virtual ICollection<PlaceMedia> PlaceMedias { get; set; }
        public virtual ICollection<PlaceItem> PlaceItems { get; set; }
        public virtual ICollection<Favorite> Favorites { get; set; }
        public virtual ICollection<Rating>  Ratings{ get; set; }
        public virtual ICollection<Review> Reviews { get; set; }

    }
}

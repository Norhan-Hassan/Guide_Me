using System.ComponentModel.DataAnnotations.Schema;

namespace Guide_Me.Models
{
    public class Place
    {
        public int Id { get; set; }

        public string PlaceName { get; set; }

        public string Category { get; set; }

        [ForeignKey("City")]
        public int  CityId { get; set; }

        public virtual City City { get; set; }
        public virtual ICollection<PlaceMedia> PlaceMedias { get; set; }

    }
}

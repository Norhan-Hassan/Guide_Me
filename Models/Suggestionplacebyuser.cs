using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Guide_Me.Models
{
    public class Suggestionplacebyuser
    {
        public int Id { get; set; }
        public string PlaceName { get; set; }
        public string? Address { get; set; }
        public double? Latitude { get; set; }

        public double? Longitude { get; set; }
        [ForeignKey("Tourist")]
        public string TouristId { get; set; }
        [Required]
       
        public virtual Tourist Tourist { get; set; }
    }
}

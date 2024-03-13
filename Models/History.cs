using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Guide_Me.Models
{
    public class History
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Place")]
        public int PlaceId { get; set; }
        [ForeignKey("Tourist")]
        public string TouristId { get; set; }
        [Required]
        public DateTime Date { get; set; }

        public virtual Place Place { get; set; }
        public virtual Tourist Tourist { get; set; }
    }
}

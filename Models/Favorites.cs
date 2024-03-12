using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Guide_Me.Models
{
    [Table ("Favorites") ]
    public class Favorites
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("tourist")]
        public string TouristID { get; set; } // Change the type to string to match ApplicationUser's primary key
        public virtual ApplicationUser tourist { get; set; }
        [ForeignKey("Place")]
        public int PlaceID { get; set; }
        public virtual Place Place { get; set; }

    }
}

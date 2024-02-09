using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Guide_Me.Models
{
    public class PlaceItem
    {
        [Key]
        public int ID { get; set; }
        public string placeItemName { get; set; }

        [ForeignKey("place")]
        public int placeID { get; set; }
        public Place place { get; set; }
        public virtual ICollection<PlaceItemMedia> PlaceItemMedias { get; set; }
    }
}

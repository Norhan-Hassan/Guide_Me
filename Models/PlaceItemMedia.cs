using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Guide_Me.Models
{
    public class PlaceItemMedia
    {
        [Key]
        public  int ID { get; set; }
        public  string MediaType { get; set; }
        public string MediaContent { get; set; }

        [ForeignKey("placeItem")]
        public int placeItemID { get; set; }

        public PlaceItem placeItem{ get; set; }
    }
}

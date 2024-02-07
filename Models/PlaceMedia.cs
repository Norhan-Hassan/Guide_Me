using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Guide_Me.Models
{
    public class PlaceMedia
    {
        public int Id { get; set; }
        public string MediaType { get; set; }
        public string MediaContent { get; set; }

        [ForeignKey("PlaceMedia")]
        public int PlaceId { get; set; }
        public virtual Place Place { get; set; }
    }
}

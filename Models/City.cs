using System.ComponentModel.DataAnnotations;

namespace Guide_Me.Models
{
    public class City
    {
        [Key]
        public int  Id { get; set; }
        [Required]
        public string CityName { get; set; }

        public  virtual ICollection<Place> Places { get; set; } 
    }
}

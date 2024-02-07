using System.ComponentModel.DataAnnotations;

namespace Guide_Me.Models
{
    public class Tourist
    {
        [Key]
        public int ID { get; set; }
        [Required]
        public string userName { get; set; }
        [Required]
        public string password { get; set; }
        [Required]
        public string email { get; set; }
        [Required]
        public string language { get; set; }
    }
}

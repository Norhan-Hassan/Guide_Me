using System.ComponentModel.DataAnnotations;

namespace Guide_Me.DTO
{
    public class TouristInfoProfileDto
    {
        [Required]
        public string userName { get; set; }

        [Required]
        public string email { get; set; }
        [Required]
        public string language { get; set; }
    }
}

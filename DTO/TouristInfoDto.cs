using System.ComponentModel.DataAnnotations;

namespace Guide_Me.DTO
{
    public class TouristInfoDto
    {
        [Required]
        public string userName { get; set; }
    
        [Required]
        public string email { get; set; }
        [Required]
        public string language { get; set; }
        [Required]
        public string newPass { get; set; }
        [Required]
        public string currentPass { get; set; }

    }
}

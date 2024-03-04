using Guide_Me.Models;
using System.ComponentModel.DataAnnotations;

namespace Guide_Me.DTO
{
    public class TouristRegisterDto
    {
        [Required]
        public string userName { get; set; }
        [Required]
        public string password { get; set; }

        [Compare("password")]
        public string ConfirmPassword { get; set; }
        [Required]
        public string email { get; set; }
        [Required]
        public string language { get; set; }

    }
}

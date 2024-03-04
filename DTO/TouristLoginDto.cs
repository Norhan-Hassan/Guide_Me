using System.ComponentModel.DataAnnotations;

namespace Guide_Me.DTO
{
    public class TouristLoginDto
    {
        [Required(ErrorMessage ="User Name Is Required")]
        public string userName { get; set; }
        [Required(ErrorMessage = "User Name Is Required")]
        public string password { get; set; }
       
    }
}

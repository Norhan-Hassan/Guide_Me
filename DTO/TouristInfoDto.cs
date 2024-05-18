using System.ComponentModel.DataAnnotations;

namespace Guide_Me.DTO
{
    public class TouristInfoDto
    {
        public string? userName { get; set; }
        public string? email { get; set; }
        public string? language { get; set; }
        public string? newPass { get; set; }
        public string? currentPass { get; set; }
        public IFormFile? Photo { get; set; }

    }
}

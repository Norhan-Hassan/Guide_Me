using System.ComponentModel.DataAnnotations;

namespace Guide_Me.DTO
{
    public class SuggestionplacebyuserDto
    {
        [Required]
        public string PlaceName { get; set; }

        
        public string? Address { get; set; }

        public double? Latitude { get; set; }

        public double? Longitude { get; set; }
    }
}

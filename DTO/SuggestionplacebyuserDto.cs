using System.ComponentModel.DataAnnotations;

namespace Guide_Me.DTO
{
    public class SuggestionplacebyuserDto
    {
        [Required]
        public string PlaceName { get; set; }

        [Required]
        public string Address { get; set; }
    }
}

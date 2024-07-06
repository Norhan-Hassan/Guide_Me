using System.ComponentModel.DataAnnotations;

namespace Guide_Me.DTO
{
    public class ReviewPlaceDto
    {
        [Required(ErrorMessage = "Place Name is required")]
        public string placeName { get; set; }
        [Required(ErrorMessage = "Tourist Name is required")]
        public string touristName { get; set; }

        [Required(ErrorMessage = "Comment is required")]
        public string comment{ get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace Guide_Me.DTO
{
    public class TouristReviewDto
    {
        public string touristName { get; set; }
        [Required(ErrorMessage = "Rating Number is required")]
        public string comment { get; set; }
    }
}

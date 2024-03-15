using System.ComponentModel.DataAnnotations;

namespace Guide_Me.DTO
{
    public class PlaceLocationDto
    {
        [Required(ErrorMessage = "Place Name Is Required")]
        public string  placeName { get; set; }

        [Required(ErrorMessage = "Latitude value Is Required")]
        public double latitude { get; set; }

        [Required(ErrorMessage = "longitude value Is Required")]
        public double longitude {  get; set; }
    }
}

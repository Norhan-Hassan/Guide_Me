using System.ComponentModel.DataAnnotations;

namespace Guide_Me.DTO
{
    public class FavouritePlacesDto
    {


        [Required(ErrorMessage = "Place Name is required")]
        public string PlaceName { get; set; }

        [Required(ErrorMessage = "Tourist Name is required")]
        public string TouristName { get; set; }
    }
}
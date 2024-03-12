using System.ComponentModel.DataAnnotations;

namespace Guide_Me.DTO { 
    public class FavouritePlacesDto
    {
        
   
            [Required(ErrorMessage = "Place ID is required")]
            public int PlaceId { get; set; }

            [Required(ErrorMessage = "Tourist ID is required")]
            public string TouristId { get; set; }
        }
    }




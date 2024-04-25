﻿using System.ComponentModel.DataAnnotations;

namespace Guide_Me.DTO
{
    public class RatePlaceDto
    {
        [Required(ErrorMessage = "Place Name is required")]
        public string placeName { get; set; }
        [Required(ErrorMessage = "Tourist Name is required")]
        public string touristName { get; set; }
        [Required(ErrorMessage = "Rating Number is required")]
        public int ratingNum { get; set; }
    }
}

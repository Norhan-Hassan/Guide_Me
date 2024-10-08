﻿using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Guide_Me.Models
{
    public class PlaceMedia
    {
        [Key]
        public int Id { get; set; }
        public string MediaType { get; set; }
        public string MediaContent { get; set; }

        [ForeignKey("Place")]
        public int PlaceId { get; set; }
        public virtual Place Place { get; set; }
    }
}

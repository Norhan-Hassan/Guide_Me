using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Guide_Me.Models
{
    public class Admin: ApplicationUser
    {
        [Key]
        public int ID { get; set; }
        public string userName { get; set; }
        public string  password { get; set; }
    }
}

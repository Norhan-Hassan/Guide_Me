using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Guide_Me.Models
{
    public class Tourist:ApplicationUser
    {
        [Key]
        public int ID { get; set; }
        [Required]
        public string userName { get; set; }
        [Required]
        public string password { get; set; }

        [Compare("Password")]
        public string ConfirmPassword { get; set; }
        [Required]
        public string email { get; set; }
        [Required]
        public string language { get; set; }
        public virtual ICollection<History> Histories { get; set; }
    }
}

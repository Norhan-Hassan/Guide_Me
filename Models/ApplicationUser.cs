using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Guide_Me.Models
{
    public class ApplicationUser:IdentityUser
    {
        [Key]
        public override string Id { get; set; }
    }
}

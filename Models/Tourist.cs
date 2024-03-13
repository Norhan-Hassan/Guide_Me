using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace Guide_Me.Models
{
    public class Tourist : IdentityUser
    {
        public string Language { get; set; }
        public virtual ICollection<Favorite> Favorites { get; set; }

    }
}

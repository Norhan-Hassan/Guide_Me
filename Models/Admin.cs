using Microsoft.EntityFrameworkCore;

namespace Guide_Me.Models
{
    public class Admin
    {
        public int ID { get; set; }
        public string userName { get; set; }
        public string  password { get; set; }
    }
}

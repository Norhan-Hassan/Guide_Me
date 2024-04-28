using Guide_Me.Models;
using Microsoft.EntityFrameworkCore;

namespace Guide_Me.Services
{
    public class TouristService : ITouristService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ApplicationDbContext _context;
        public TouristService(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public string GetUserIdByUsername(string username)
        {
            var user = _context.Users.FirstOrDefault(u => u.UserName == username);

            return user != null ? user.Id : null;
        }
        public string GetUserNameByUserId(string userId)
        {
            var users = _context.Users.Where(u => u.Id == userId).ToList();
            var user = users.FirstOrDefault();
            return user != null ? user.UserName : null;
        }
       
        
    }
}

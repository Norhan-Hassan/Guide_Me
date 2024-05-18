using Guide_Me.DTO;
using Guide_Me.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Guide_Me.Services
{
    public class TouristService : ITouristService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<Tourist> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public TouristService(UserManager<Tourist> userManager, ApplicationDbContext context, IHttpContextAccessor httpContextAccessor, IWebHostEnvironment hostingEnvironment)
        {
            _context = context;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
            _hostingEnvironment = hostingEnvironment;
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

        public TouristInfoProfileDto GetStudentInfo(string name)
        {
            var tourist = _context.Tourist.FirstOrDefault(t => t.UserName == name);

            if (tourist != null)
            {
                var touristInfo = new TouristInfoProfileDto
                {
                    userName = name,
                    email = tourist.Email,
                    language = tourist.Language,
                    PhotoUrl = string.IsNullOrEmpty(tourist.PhotoPath) ? null : $"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}/uploads/photos/{Path.GetFileName(tourist.PhotoPath)}"
                };
                return touristInfo;
            }
            return null;
        }

        public async Task UpdateUserInfo(string Name, TouristInfoDto infoDto)
        {
            var tourist = await _context.Tourist.FirstOrDefaultAsync(t => t.UserName == Name);
            if (tourist == null)
            {
                throw new Exception("Tourist not found.");
            }

            if (!string.IsNullOrEmpty(infoDto.userName))
            {
                var checkExistUser = await _context.Tourist.FirstOrDefaultAsync(t => t.UserName == infoDto.userName && t.Id != tourist.Id);
                if (checkExistUser != null)
                {
                    throw new Exception("Try another name, this name is already taken");
                }
                tourist.UserName = infoDto.userName;
            }

            if (!string.IsNullOrEmpty(infoDto.email))
            {
                var checkExistUser = await _context.Tourist.FirstOrDefaultAsync(t => t.Email == infoDto.email && t.Id != tourist.Id);
                if (checkExistUser != null)
                {
                    throw new Exception("Not able to update this email, it is already taken");
                }
                tourist.Email = infoDto.email;
            }

            if (!string.IsNullOrEmpty(infoDto.language))
            {
                tourist.Language = infoDto.language;
            }

            if (!string.IsNullOrEmpty(infoDto.newPass) || !string.IsNullOrEmpty(infoDto.currentPass))
            {
                if (string.IsNullOrEmpty(infoDto.newPass) || string.IsNullOrEmpty(infoDto.currentPass))
                {
                    throw new Exception("Both current password and new password are required to change password.");
                }

                var passwordCheck = await _userManager.CheckPasswordAsync(tourist, infoDto.currentPass);
                if (!passwordCheck)
                {
                    throw new Exception("Current password is incorrect");
                }

                var result = await _userManager.ChangePasswordAsync(tourist, infoDto.currentPass, infoDto.newPass);
                if (!result.Succeeded)
                {
                    throw new Exception("Failed to change password");
                }
            }

            if (infoDto.Photo != null && infoDto.Photo.Length > 0)
            {
                var uploadsFolder = Path.Combine(_hostingEnvironment.WebRootPath, "uploads", "photos");
                var uniqueFileName = $"{Guid.NewGuid()}_{infoDto.Photo.FileName}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await infoDto.Photo.CopyToAsync(fileStream);
                }

                tourist.PhotoPath = Path.Combine("uploads", "photos", uniqueFileName);
            }

            await _context.SaveChangesAsync();
        }




    }
}

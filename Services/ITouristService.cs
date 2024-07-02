using Guide_Me.DTO;
using Guide_Me.Models;

namespace Guide_Me.Services
{
    public interface ITouristService
    {
        string GetUserIdByUsername(string username);
        string GetUserNameByUserId(string userId);
        TouristInfoProfileDto GetStudentInfo(string username);
        Task UpdateUserInfo(string Name, TouristInfoDto infoDto);
        string GetUserPhotoByUserId(string userId);
        Tourist GetTouristById(string userId);
        Tourist GetTouristByUsername(string username);
    }
}

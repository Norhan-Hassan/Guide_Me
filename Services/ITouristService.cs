using Guide_Me.DTO;

namespace Guide_Me.Services
{
    public interface ITouristService
    {
        string GetUserIdByUsername(string username);
        string GetUserNameByUserId(string userId);
        TouristInfoProfileDto GetStudentInfo(string username);
        Task UpdateUserInfo(string Name, TouristInfoDto infoDto);
    }
}

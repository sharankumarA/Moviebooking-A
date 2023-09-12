using MovieBooking.API.Models.DTO;

namespace MovieBooking.API.Interfaces.IBusiness
{
    public interface IUserBusiness
    {
        public Task<string> AddUser(UserDto user);
        public Task<string> GetUserToken(string loginId, string password);
        public Task<string> ChangePassword(string loginId, string newPassword);
    }
}

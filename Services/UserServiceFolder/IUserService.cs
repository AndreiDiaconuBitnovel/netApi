using WebApplication2.DataDB;

namespace WebApplication2.Services.UserService
{
    public interface IUserService
    {
        Task<List<User>> GetUsersByEmailOrUsername(string email, string username);
        Task<List<User>> GetUsersByUsername(string username);
        Task<User> SaveUserAndImg(string email, string username, byte[] data);
    }
}

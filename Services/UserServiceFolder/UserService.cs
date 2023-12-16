using Microsoft.EntityFrameworkCore;
using WebApplication2.DataDB;
using WebApplication2.Services.UserService;

namespace WebApplication2.Services.UserServiceFolder
{
    public class UserService: IUserService
    {
        private readonly TeamDropDatabaseContext _testContext;

        public UserService(TeamDropDatabaseContext testContext)
        {
            _testContext = testContext;
        }
        public async Task<List<User>> GetUsersByEmailOrUsername(string email, string username)
        {
            return await _testContext.Users.Where(x=>x.Username==username || x.Email==email).ToListAsync();
        }

        public async Task<User> SaveUserAndImg(string email, string username, byte[] data)
        {
            try
            {
                var image = new Image()
                {
                    Id = Guid.NewGuid(),
                    ImageObj = data
                };
                await _testContext.Images.AddAsync(image);
                var userDetails = new User()
                {
                    Email = email,
                    Username = username,
                    IdUser= Guid.NewGuid(),
                    IdImg= image.Id
                };
                await _testContext.Users.AddAsync(userDetails);
                await _testContext.SaveChangesAsync();


                return userDetails;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}

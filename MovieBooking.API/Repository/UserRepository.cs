using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MovieBooking.API.Interfaces.IRepository;
using MovieBooking.API.Models;
using MovieBooking.API.Models.Appsettings;

namespace MovieBooking.API.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly IOptions<MongoDbConfig> _mongoDbConfig;
        private readonly IMongoCollection<User> _users;
        private readonly ILogger<UserRepository> _logger;

        public UserRepository(IOptions<MongoDbConfig> mongoDbConfig, IMongoClient mongoClient, ILogger<UserRepository> logger)
        {
            _mongoDbConfig = mongoDbConfig;

            var database = mongoClient.GetDatabase(_mongoDbConfig.Value.DatabaseName);
            _users = database.GetCollection<User>(_mongoDbConfig.Value.UserCollectionName);
            _logger = logger;
        }

        public async Task<bool> AddUser(User user)
        {
            _logger.LogInformation("Add user to role : user repository");

            try
            {
                await _users.InsertOneAsync(user);
                return true;
            }
            catch(Exception)
            {
                return false;
            }
        }

       public async Task DeleteUser(string id)
        {
            await _users.DeleteOneAsync(user => user.Id == id);
        }

        public async Task<User> GetUser(string id)
        {
            _logger.LogInformation("Find user by id: user repository");

            var users = await _users.FindAsync(user => user.Id == id);
            return await users.FirstOrDefaultAsync();
        }

        public async Task<User> GetUser(string loginId, string email)
        {
            _logger.LogInformation("Get user by loginId and email : user repository");

            var users = await _users.FindAsync(user => user.LoginId == loginId || user.Email == email);
            return await users.FirstOrDefaultAsync();
        }

        public async Task<User> GetUserByLoginIdPassword(string loginId, string password)
        {
            _logger.LogInformation("GetUserByLoginIdPassword: user repository");

            var users = await _users.FindAsync(user => user.LoginId == loginId && user.Password == password);
            return await users.FirstOrDefaultAsync();
        }

        public async Task<User> GetUserByLoginId(string loginId)
        {
            _logger.LogInformation("GetUserByLoginId: user repository");

            var users = await _users.FindAsync(user => user.LoginId.Equals(loginId));
            return await users.FirstOrDefaultAsync();
        }

        public async Task<List<User>> GetUsers()
        {
            var users = await _users.FindAsync(user => true);
            return users.ToList();
        }

        public async Task<bool> UpdateUser(User user)
        {
            try
            {
                await _users.ReplaceOneAsync(u => u.Id == user.Id, user);
                return true;
            }
            catch(Exception)
            {
                return false;
            }
        }
    }
}

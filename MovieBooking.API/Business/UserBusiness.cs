using AutoMapper;
using MovieBooking.API.Interfaces.IBusiness;
using MovieBooking.API.Interfaces.IRepository;
using MovieBooking.API.Models;
using MovieBooking.API.Models.DTO;
using System.Security.Cryptography;

namespace MovieBooking.API.Business
{
    public class UserBusiness : IUserBusiness
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IIdentityBusiness _identityBusiness;
        private readonly ILogger<UserBusiness> _logger;

        public UserBusiness(IUserRepository userRepository, IMapper mapper, IIdentityBusiness identityBusiness, ILogger<UserBusiness> logger)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _identityBusiness = identityBusiness;
            _logger = logger;
        }

        public async Task<string> AddUser(UserDto user)
        {

            _logger.LogInformation("Add new user : User Business");

            string userId = string.Empty;

            try
            {

                var existingUser = await _userRepository.GetUser(user.LoginId, user.Email);

                if(existingUser is null || string.IsNullOrEmpty(existingUser?.Id))
                {
                    _identityBusiness.CreatePasswordHashSalt(user.Password, out byte[] passwordHash, out byte[] passwordSalt);
                    userId = Guid.NewGuid().ToString();
                    var userModel = _mapper.Map<User>(user);
                    userModel.Id = userId;
                    userModel.PasswordHash = passwordHash;
                    userModel.PasswordSalt = passwordSalt;
                    var isInserted = await _userRepository.AddUser(userModel);

                    if(!isInserted)
                    {
                        userId = string.Empty;
                    }
                }
            }
            catch(Exception)
            {
                userId = string.Empty;
            }

            return userId;
        }

        public async Task<string> GetUserToken(string loginId, string password)
        {
            _logger.LogInformation("Get user token : User Business");

            string token = string.Empty;
            try
            {

                var existingUserModel = await _userRepository.GetUserByLoginIdPassword(loginId, password);
                if(existingUserModel is not null)
                {
                    var isAuthorized = _identityBusiness.AuthorizeUser(existingUserModel.Password, existingUserModel.PasswordHash, existingUserModel.PasswordSalt);

                    if (isAuthorized)
                    {
                        token = _identityBusiness.CreateToken(existingUserModel);
                    }
                }
            }
            catch (Exception)
            {
                token = string.Empty;
            }

            return token;
        }

        public async Task<string> ChangePassword(string loginId, string newPassword)
        {
            _logger.LogInformation("Change password : User Business");

            string status = string.Empty;
            try
            {
                var existingUserModel = await _userRepository.GetUserByLoginId(loginId);
                if(existingUserModel is not null)
                {
                    _identityBusiness.CreatePasswordHashSalt(newPassword, out byte[] newPasswordHash, out byte[] newPasswordSalt);
                    existingUserModel.Password = newPassword;
                    existingUserModel.PasswordHash = newPasswordHash;
                    existingUserModel.PasswordSalt = newPasswordSalt;

                    var isUpdateSuccess = await _userRepository.UpdateUser(existingUserModel);

                    if(isUpdateSuccess)
                    {
                        status = "Password changed successfully";
                    }
                }
                else
                {
                    status = $"Incorrect username {loginId}";
                }
            }
            catch(Exception)
            {
                status = string.Empty;
            }

            return status;
        }
    }
}

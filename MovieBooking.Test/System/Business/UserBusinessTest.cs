using AutoMapper;
using Moq;
using MovieBooking.API.Business;
using MovieBooking.API.Interfaces.IBusiness;
using MovieBooking.API.Interfaces.IRepository;
using MovieBooking.API.Models;
using MovieBooking.Test.Fixtures;
using MovieBooking.Test.Setups;
using System;
using System.Threading.Tasks;
using Xunit;

namespace MovieBooking.Test.System.Business
{
    public class UserBusinessTest
    {
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly Mock<IIdentityBusiness> _mockIdentityBusiness;

        private readonly IMapper _mapper;
        private readonly UserBusiness _userBusiness;

        public UserBusinessTest()
        {
            _mockUserRepository = new();
            _mockIdentityBusiness = new();

            _mapper = Setup.SetupAutoMapper();
            _userBusiness = new(_mockUserRepository.Object, _mapper, _mockIdentityBusiness.Object);
        }

        [Fact]
        public async Task AddUser_Success()
        {
            var user = UserFixture.ReturnUser();
            _mockUserRepository.Setup(mock => mock.GetUser(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(new User()));
            _mockUserRepository.Setup(mock => mock.AddUser(It.IsAny<User>())).Returns(Task.FromResult(true));
            
            var userId = await _userBusiness.AddUser(user);

            Assert.NotEmpty(userId);
        }

        [Fact]
        public async Task AddUser_BadRequest_UserAlreadyExists()
        {
            var user = UserFixture.ReturnUser();
            _mockUserRepository.Setup(mock => mock.GetUser(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(new User() { Id = Guid.NewGuid().ToString()}));
            
            var userId = await _userBusiness.AddUser(user);

            Assert.Empty(userId);
        }

        [Fact]
        public async Task GetUser_Success()
        {
            var loginPass = UserFixture.ReturnLoginIdPassword();
            var loginId = loginPass.Item1;
            var password = loginPass.Item2;

            var user = await _userBusiness.GetUserToken(loginId, password);

            Assert.NotNull(user);
        }
    }
}

using System.Collections.ObjectModel;
using System.Threading.Tasks;
using JWTAPI.Core.Models;
using JWTAPI.Core.Repositories;
using JWTAPI.Core.Security.Hashing;
using JWTAPI.Core.Services;
using JWTAPI.Services;
using Moq;
using Xunit;

namespace JWTPAPI.Tests.Services
{
    public class UserServiceTests
    {
        private Mock<IPasswordHasher> _passwordHasher;
        private Mock<IUserRepository> _userRepository;
        private Mock<IUnitOfWork> _unitOfWork;

        private IUserService _userService;

        public UserServiceTests()
        {
            SetupMocks();
            _userService = new UserService(_userRepository.Object, _unitOfWork.Object, _passwordHasher.Object);
        }

        private void SetupMocks()
        {
            _passwordHasher = new Mock<IPasswordHasher>();
            _passwordHasher.Setup(ph => ph.HashPassword(It.IsAny<string>())).Returns("123");

            _userRepository = new Mock<IUserRepository>();
            _userRepository.Setup(r => r.FindByEmailAsync("test@test.com"))
                .ReturnsAsync(new User { Id = 1, Email = "test@test.com", UserRoles = new Collection<UserRole>() });

            _userRepository.Setup(r => r.FindByEmailAsync("secondtest@secondtest.com"))
                .Returns(Task.FromResult<User>(null));

            _userRepository.Setup(r => r.AddAsync(It.IsAny<User>(), It.IsAny<ERole[]>())).Returns(Task.CompletedTask);

            _unitOfWork = new Mock<IUnitOfWork>();
            _unitOfWork.Setup(u => u.CompleteAsync()).Returns(Task.CompletedTask);
        }

        [Fact]
        public async Task Should_Create_Non_Existing_User()
        {
            var user = new User { Email = "mytestuser@mytestuser.com", Password = "123", UserRoles = new Collection<UserRole>() };
            
            var response = await _userService.CreateUserAsync(user, ERole.Common);

            Assert.NotNull(response);
            Assert.True(response.Success);
            Assert.Equal(user.Email, response.User.Email);
            Assert.Equal(user.Password, response.User.Password);
        }

        [Fact]
        public async Task Should_Not_Create_User_When_Email_Is_Alreary_In_Use()
        {
            var user = new User { Email = "test@test.com", Password = "123", UserRoles = new Collection<UserRole>() };
        
            var response = await _userService.CreateUserAsync(user, ERole.Common);

            Assert.False(response.Success);
            Assert.Equal("Email already in use.", response.Message);
        }

        [Fact]
        public async Task Should_Find_Existing_User_By_Email()
        {
            var user = await _userService.FindByEmailAsync("test@test.com");
            Assert.NotNull(user);
            Assert.Equal("test@test.com", user.Email);
        }

        [Fact]
        public async Task Should_Return_Null_When_Trying_To_Find_User_By_Invalid_Email()
        {
            var user = await _userService.FindByEmailAsync("secondtest@secondtest.com");
            Assert.Null(user);
        }
    }
}
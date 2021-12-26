using JWTAPI.Core.Models;
using JWTAPI.Core.Repositories;
using JWTAPI.Core.Security.Hashing;
using JWTAPI.Core.Services;
using JWTAPI.Core.Services.Communication;

namespace JWTAPI.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordHasher _passwordHasher;

        public UserService(IUserRepository userRepository, IUnitOfWork unitOfWork, IPasswordHasher passwordHasher)
        {
            _passwordHasher = passwordHasher;
            _unitOfWork = unitOfWork;
            _userRepository = userRepository;
        }

        public async Task<CreateUserResponse> CreateUserAsync(User user, params ApplicationRole[] userRoles)
        {
            var existingUser = await _userRepository.FindByEmailAsync(user.Email);
            if(existingUser != null)
            {
                return new CreateUserResponse(false, "Email already in use.", null);
            } 

            user.Password = _passwordHasher.HashPassword(user.Password);

            await _userRepository.AddAsync(user, userRoles);
            await _unitOfWork.CompleteAsync();

            return new CreateUserResponse(true, null, user);
        }

        public async Task<User> FindByEmailAsync(string email)
        {
            return await _userRepository.FindByEmailAsync(email);
        }
    }
}
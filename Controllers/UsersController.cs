using System.Threading.Tasks;
using AutoMapper;
using JWTAPI.Controllers.Resources;
using JWTAPI.Core.Models;
using JWTAPI.Core.Repositories;
using JWTAPI.Core.Security.Hashing;
using Microsoft.AspNetCore.Mvc;

namespace JWTAPI.Controllers
{
    [Route("/api/[controller]")]
    public class UsersController : Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOrWork;
        private readonly IMapper _mapper;
        private readonly IPasswordHasher _passwordHasher;

        public UsersController(IUserRepository userRepository, IUnitOfWork unitOrWork, IMapper mapper, IPasswordHasher passwordHasher)
        {
            _passwordHasher = passwordHasher;
            _mapper = mapper;
            _unitOrWork = unitOrWork;
            _userRepository = userRepository;
        }

        [HttpPost]
        public async Task<IActionResult> CreateUserAsync([FromBody] UserCredentialsResource userCredentials)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            var user = _mapper.Map<UserCredentialsResource, User>(userCredentials);
            user.Password = _passwordHasher.HashPassword(user.Password);

            await _userRepository.AddAsync(user, new[] { ERole.Common });
            await _unitOrWork.CompleteAsync();

            var userResource = _mapper.Map<User, UserResource>(user);
            return Ok(userResource);
        }
    }
}
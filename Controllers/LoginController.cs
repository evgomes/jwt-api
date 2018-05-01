using System.Threading.Tasks;
using AutoMapper;
using JWTAPI.Controllers.Resources;
using JWTAPI.Core.Repositories;
using JWTAPI.Core.Security.Hashing;
using JWTAPI.Core.Security.Tokens;
using Microsoft.AspNetCore.Mvc;

namespace JWTAPI.Controllers
{
    public class AuthController : Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ITokenHandler _tokenHandler;

        public AuthController(IUserRepository userRepository, IMapper mapper, IPasswordHasher passwordHasher, ITokenHandler tokenHandler)
        {
            _mapper = mapper;
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _tokenHandler = tokenHandler;
        }

        [Route("/api/login")]
        [HttpPost]
        public async Task<IActionResult> LoginAsync([FromBody] UserCredentialsResource userCredentials)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userRepository.FindAsync(userCredentials.Email);

            if (user == null)
            {
                return NotFound();
            }

            if (!_passwordHasher.PasswordMatches(userCredentials.Password, user.Password))
            {
                return BadRequest();
            }

            var token = _tokenHandler.CreateAccessToken(user);
            var accessTokenResource = _mapper.Map<AccessToken, AccessTokenResource>(token);

            return Ok(accessTokenResource);
        }

        [Route("/api/token/refresh")]
        [HttpPost]
        public async Task<IActionResult> RefreshTokenAsync([FromBody] RefreshTokenResource refreshTokenResource)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var refreshToken = _tokenHandler.TakeRefreshToken(refreshTokenResource.Token);

            if(refreshToken == null)
            {
                return BadRequest("Invalid refresh token.");
            }

            if (refreshToken.IsExpired())
            {
                return BadRequest("Expired refresh token.");
            }

            var user = await _userRepository.FindAsync(refreshTokenResource.UserEmail);
            if(user == null)
            {
                _tokenHandler.RevokeRefreshToken(refreshToken.Token);
                return BadRequest("Invalid refresh token.");
            }

            var token = _tokenHandler.CreateAccessToken(user);
            var tokenResource = _mapper.Map<AccessToken, AccessTokenResource>(token);

            return Ok(tokenResource);
        }

        [Route("/api/token/revoke")]
        [HttpPost]
        public IActionResult RevokeToken([FromBody] RevokeTokenResource revokeTokenResource)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _tokenHandler.RevokeRefreshToken(revokeTokenResource.Token);

            return NoContent();
        }
    }
}
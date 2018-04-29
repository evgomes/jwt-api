using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using JWTAPI.Controllers.Resources;
using JWTAPI.Models;
using JWTAPI.Models.Repositories;
using JWTAPI.Models.Security;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace JWTAPI.Controllers
{
    public class AuthController : Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly TokenOptions _tokenOptions;
        private readonly SigningConfigurations _signingConfigurations;
        private readonly IPasswordHasher _passwordHasher;

        public AuthController(
            IUserRepository userRepository,
            IMapper mapper,
            SigningConfigurations signingConfigurations,
            IOptionsSnapshot<TokenOptions> tokenOptionsSnapshot,
            IPasswordHasher passwordHasher)
        {
            _mapper = mapper;
            _userRepository = userRepository;
            _tokenOptions = tokenOptionsSnapshot.Value;
            _signingConfigurations = signingConfigurations;
            _passwordHasher = passwordHasher;
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

            var handler = new JwtSecurityTokenHandler();
            var securityToken = new JwtSecurityToken(
                issuer : _tokenOptions.Issuer,
                audience : _tokenOptions.Audience,
                claims : GetUserClaims(user),
                expires : DateTime.UtcNow.AddSeconds(_tokenOptions.Expiration),
                notBefore : DateTime.UtcNow,
                signingCredentials : _signingConfigurations.SigningCredentials
            );
            var token = handler.WriteToken(securityToken);

            var response = new { token = token };
            return Ok(response);
        }

        private IEnumerable<Claim> GetUserClaims(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            foreach(var userRole in user.UserRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, userRole.Role.Name));
            }

            return claims;
        }
    }
}
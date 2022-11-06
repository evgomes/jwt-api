namespace JWTAPI.Controllers;

[ApiController]
[Route("api/")]
public class AuthController : ControllerBase
{
    private readonly IMapper _mapper;
    private readonly IAuthenticationService _authenticationService;

    public AuthController(IMapper mapper, IAuthenticationService authenticationService)
    {
        _authenticationService = authenticationService;
        _mapper = mapper;
    }

    [HttpPost("login")]
    public async Task<IActionResult> LoginAsync(
        [FromBody] UserCredentialsResource userCredentials)
    {
        var response = await _authenticationService
            .CreateAccessTokenAsync(userCredentials.Email, userCredentials.Password);

        if (!response.Success)
        {
            return BadRequest(response.Message);
        }

        var accessTokenResource = _mapper.Map<AccessToken, AccessTokenResource>(response.Token);

        return Ok(accessTokenResource);
    }

    [HttpPost("token/refresh")]
    public async Task<IActionResult> RefreshTokenAsync([FromBody] RefreshTokenResource refreshTokenResource)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var response = await _authenticationService.RefreshTokenAsync(refreshTokenResource.Token, refreshTokenResource.UserEmail);
        if (!response.Success)
        {
            return BadRequest(response.Message);
        }

        var tokenResource = _mapper.Map<AccessToken, AccessTokenResource>(response.Token);
        return Ok(tokenResource);
    }

    [HttpPost("token/revoke")]
    public IActionResult RevokeToken([FromBody] RevokeTokenResource resource)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        _authenticationService.RevokeRefreshToken(resource.Token, resource.Email);
        return NoContent();
    }
}
namespace JWTAPI.Controllers;

[ApiController]
[Route("/api/users")]
public class UsersController : ControllerBase
{
    private readonly IMapper _mapper;
    private readonly IUserService _userService;

    public UsersController(IUserService userService, IMapper mapper)
    {
        _userService = userService;
        _mapper = mapper;
    }

    [HttpPost]
    public async Task<IActionResult> CreateUserAsync(
        [FromBody] UserCredentialsResource userCredentials)
    {
        var user = _mapper.Map<UserCredentialsResource, User>(userCredentials);

        var response = await _userService.CreateUserAsync(user, ApplicationRole.Common);

        if (!response.Success)
        {
            return BadRequest(response.Message);
        }

        var userResource = _mapper.Map<User, UserResource>(response.User);

        return Ok(userResource);
    }
}
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
	public async Task<IActionResult> CreateUserAsync([FromBody] UserCredentialsResource userCredentials)
	{
		var user = _mapper.Map<User>(userCredentials);

		var response = await _userService.CreateUserAsync(user, ApplicationRole.Common);
		if (!response.Success)
		{
			return BadRequest(response.Message);
		}

		return Ok(_mapper.Map<UserResource>(response.User));
	}
}
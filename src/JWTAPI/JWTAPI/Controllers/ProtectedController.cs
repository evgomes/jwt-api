namespace JWTAPI.Controllers;

[ApiController]
[Route("api/protected")]
public class ProtectedController : ControllerBase
{
    [HttpGet]
    [Authorize]
    [Route("for-commonusers")]
    public IActionResult GetProtectedData()
    {
        return Ok("Hello world from protected controller.");
    }

    [HttpGet]
    [Authorize(Roles = "Administrator")]
    [Route("for-administrators")]
    public IActionResult GetProtectedDataForAdmin()
    {
        return Ok("Hello admin!");
    }
}
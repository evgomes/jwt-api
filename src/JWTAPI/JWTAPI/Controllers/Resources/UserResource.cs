namespace JWTAPI.Controllers.Resources;

public record UserResource
{
    public int Id { get; set; }
    public string Email { get; set; } = null!;
    public IEnumerable<string> Roles { get; set; } = new List<string>();
}
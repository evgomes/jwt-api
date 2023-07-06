namespace JWTAPI.Controllers.Resources;

public class RevokeTokenResource
{
    [Required]
    public string? Token { get; init; }

    [Required]
    public string? Email { get; init; }
}
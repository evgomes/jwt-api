namespace JWTAPI.Controllers.Resources;

public class UserCredentialsResource
{
    [Required]
    [EmailAddress]
    [StringLength(255)]
    public string? Email { get; init; }

    [Required]
    [StringLength(32)]
    public string? Password { get; init; }
}
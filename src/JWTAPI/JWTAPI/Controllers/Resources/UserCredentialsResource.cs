namespace JWTAPI.Controllers.Resources;
public class UserCredentialsResource
{
    [Required]
    [EmailAddress]
    [StringLength(255)]
    public string Email { get; set; }

    [Required]
    [StringLength(32)]
    public string Password { get; set; }
}
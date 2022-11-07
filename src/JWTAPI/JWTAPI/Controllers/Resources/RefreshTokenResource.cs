namespace JWTAPI.Controllers.Resources;
public class RefreshTokenResource
{
    [Required]
    public string Token { get; set; }

    [Required]
    [EmailAddress]
    [StringLength(255)]
    public string UserEmail { get; set; }
}
namespace JWTAPI.Core.Models;
public class User
{
    public int Id { get; set; }

    [Required]
    [EmailAddress]
    [StringLength(255)]
    public string Email { get; set; }

    [Required]
    public string Password { get; set; }

    public ICollection<UserRole> UserRoles { get; set; } = new Collection<UserRole>();
}
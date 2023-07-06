namespace JWTAPI.Core.Models;
public class Role
{
    public int Id { get; set; }

    [Required]
    [StringLength(50)]
    public string Name { get; set; } = null!;

    public virtual ICollection<UserRole> UsersRole { get; set; } = new Collection<UserRole>();
}
using Microsoft.AspNetCore.Identity;

namespace ECommerce.Infrastructure.Identity;

public class ApplicationRole:IdentityRole<Guid>
{
    public ApplicationRole() => Id = Guid.NewGuid();
    public ApplicationRole(string roleName):this()=>Name = roleName;

    public string? Description { get; set; }
}

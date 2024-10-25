using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

public class Role : IdentityRole
{
    public string Description { get; set; }
    public ICollection<Permission> Permissions { get; set; }
}

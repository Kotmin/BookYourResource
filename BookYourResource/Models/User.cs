using Microsoft.AspNetCore.Identity;

public class User : IdentityUser
{
    [PersonalData]
    public string DisplayName { get; set; }

}

using Microsoft.AspNetCore.Identity;

public class User : IdentityUser
{
    [PersonalData]
    public string DisplayName { get; set; }

    public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();

}

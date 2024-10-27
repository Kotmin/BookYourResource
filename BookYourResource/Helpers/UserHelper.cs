using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

public static class UserHelper
{
    public static async Task ChangeUserPasswordAsync(IServiceProvider serviceProvider, string userId, string newPassword)
    {
        var userManager = serviceProvider.GetRequiredService<UserManager<User>>();
        
        var user = await userManager.FindByIdAsync(userId);
        if (user != null)
        {
            var token = await userManager.GeneratePasswordResetTokenAsync(user);
            await userManager.ResetPasswordAsync(user, token, newPassword);
        }
    }
}

using Domain.Constants;
using Microsoft.AspNetCore.Identity;

namespace Persistence.Data;

public static class RoleSeeder
{
    public static async Task SeedAsync(RoleManager<IdentityRole<int>> roleManager)
    {
        var roles = new[] { RoleNames.Admin, RoleNames.User };

        foreach (var roleName in roles)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new IdentityRole<int>(roleName));
            }
        }
    }
}

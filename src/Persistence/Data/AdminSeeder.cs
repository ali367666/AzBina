using Application.Options;
using Domain.Constants;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace Persistence.Data;

public static class AdminSeeder
{
    public static async Task SeedAsync(UserManager<User> userManager, IOptions<SeedOptions> seedOptions)
    {
        var seed = seedOptions.Value;

        if (string.IsNullOrWhiteSpace(seed.AdminEmail) ||
            string.IsNullOrWhiteSpace(seed.AdminPassword))
            return;

        var existing = await userManager.FindByEmailAsync(seed.AdminEmail);

        if (existing is not null)
        {
            if (!await userManager.IsInRoleAsync(existing, RoleNames.Admin))
                await userManager.AddToRoleAsync(existing, RoleNames.Admin);

            if (!existing.EmailConfirmed)
            {
                existing.EmailConfirmed = true;
                await userManager.UpdateAsync(existing);
            }

            return;
        }

        var admin = new User
        {
            UserName = seed.AdminEmail,
            Email = seed.AdminEmail,
            FullName = string.IsNullOrWhiteSpace(seed.AdminFullName) ? "Admin" : seed.AdminFullName,
            EmailConfirmed = true
        };

        var create = await userManager.CreateAsync(admin, seed.AdminPassword);
        if (!create.Succeeded)
            return;

        await userManager.AddToRoleAsync(admin, RoleNames.Admin);
    }
}

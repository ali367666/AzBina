using Application.Options;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Persistence.Data;

namespace API.Extensions;

public static class WebApplicationExtensions
{
    // Sync wrapper (istəsən Program.cs sadə qalsın)
    public static WebApplication ConfigurePipeline(this WebApplication app)
    {
        app.ConfigurePipelineAsync().GetAwaiter().GetResult();
        return app;
    }

    public static async Task<WebApplication> ConfigurePipelineAsync(this WebApplication app)
    {
        // 7.2 Middleware sırası (tipik)
        // Exception handling (əgər custom middleware varsa buraya qoy)
        // app.UseExceptionHandler("/error"); // varsa

        app.UseStaticFiles();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();
        app.MapFallbackToFile("index.html");

        // 7.1 Seed (app start olanda 1 dəfə)
        using (var scope = app.Services.CreateScope())
        {
            var sp = scope.ServiceProvider;

            var roleManager = sp.GetRequiredService<RoleManager<IdentityRole<int>>>();
            await RoleSeeder.SeedAsync(roleManager);

            if (app.Environment.IsDevelopment())
            {
                var userManager = sp.GetRequiredService<UserManager<User>>();
                var seedOptions = sp.GetRequiredService<IOptions<SeedOptions>>();
                await AdminSeeder.SeedAsync(userManager, seedOptions);
            }
        }

        return app;
    }
}

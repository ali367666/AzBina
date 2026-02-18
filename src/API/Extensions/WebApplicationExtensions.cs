using API.Middlewares;
using Application.Options;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Persistence.Data;

namespace API.Extensions;

public static class WebApplicationExtensions
{
    public static WebApplication ConfigurePipeline(this WebApplication app)
    {
        app.ConfigurePipelineAsync().GetAwaiter().GetResult();
        return app;
    }

    public static async Task<WebApplication> ConfigurePipelineAsync(this WebApplication app)
    {
        app.UseMiddleware<ExceptionMiddleware>();

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

        using (var scope = app.Services.CreateScope())
        {
            var sp = scope.ServiceProvider;

            var roleManager = sp.GetRequiredService<RoleManager<IdentityRole<int>>>();
            await RoleSeeder.SeedAsync(roleManager);

            var userManager = sp.GetRequiredService<UserManager<User>>();
            var seedOptions = sp.GetRequiredService<IOptions<SeedOptions>>();
            await AdminSeeder.SeedAsync(userManager, seedOptions);
        }

        return app;
    }
}

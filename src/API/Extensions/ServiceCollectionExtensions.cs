using API.Options;
using Application.Abstracts.Repositories;
using Application.Abstracts.Services;
using Application.Options;
using Application.Validations.Auth;
using Domain.Entities;
using FluentValidation;
using Infrastructure.Extensions;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Persistence.Context;
using Persistence.Repositories;
using Persistence.Services;

namespace API.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // 0) MinIO
        services.AddMinioStorage(configuration);
        services.AddScoped<IFileStorageService, S3MinioFileStorageService>();

        // 1) Identity + EF Stores (INT KEY)
        services.AddIdentity<User, IdentityRole<int>>(options =>
        {
            options.User.RequireUniqueEmail = true;

            options.Password.RequiredLength = 8;
            options.Password.RequireDigit = true;
            options.Password.RequireUppercase = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireNonAlphanumeric = true;
        })
        .AddEntityFrameworkStores<BinaDbContext>()
        .AddDefaultTokenProviders();

        // 2) JwtOptions bind
        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));

        // 3) Authentication + JwtBearer
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer();

        // 4) JwtBearerOptions config
        services.ConfigureOptions<ConfigureJwtBearerOptions>();

        // 5) FluentValidation
        services.AddValidatorsFromAssemblyContaining<RegisterRequestValidator>();
        services.AddValidatorsFromAssemblyContaining<LoginRequestValidator>();

        // 6) Auth / Token services
        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
        services.AddScoped<IAuthService, AuthService>();

        // 7) Refresh token repository + service
        
        //services.AddScoped<IRefreshTokenRepository, RefreshTokenService>();



        return services;
    }
}

using API.Extensions;
using Application.Abstracts.Repositories;
using Application.Abstracts.Services;
using Application.DTOs.CityDTOs.RequestDTOs;
using Application.DTOs.DistrictDTOs.RequestDTOs;
using Application.DTOs.MediaPropertyDTOs.RequestDTOs;
using Application.DTOs.PropertyListeningDTOs.RequestDTOs;
using Application.Options;
using Application.Shared.Helpers.Responses;
using Application.Validations.Auth;
using Application.Validations.CityValidation;
using Application.Validations.DistrictValidation;
using Application.Validations.MediaPropertyValidation;
using Application.Validations.PropertyListingValidation;
using Domain.Entities;
using FluentValidation;
using FluentValidation.AspNetCore;
using Infrastructure.Extensions;
using Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Persistence.Context;
using Persistence.Repositories;
using Persistence.Services;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

#region Controllers + JSON + FluentValidation
builder.Services
    .AddControllers()
    .AddJsonOptions(o => o.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles)
    .AddFluentValidation(fv =>
    {
        fv.RegisterValidatorsFromAssemblyContaining<CreateDistrictValidation>();
        fv.RegisterValidatorsFromAssemblyContaining<CreateCityValidation>();
        fv.RegisterValidatorsFromAssemblyContaining<CreatePropertyListingValidation>();
        fv.RegisterValidatorsFromAssemblyContaining<CreateMediaPropertyValidator>();
        fv.RegisterValidatorsFromAssemblyContaining<RegisterRequestValidator>();
        fv.RegisterValidatorsFromAssemblyContaining<LoginRequestValidator>();
    });

builder.Services.AddScoped<IValidator<DistrictCreateDTO>, CreateDistrictValidation>();
builder.Services.AddScoped<IValidator<CreateCityDTOs>, CreateCityValidation>();
builder.Services.AddScoped<IValidator<CreatePropertyListing>, CreatePropertyListingValidation>();
builder.Services.AddScoped<IValidator<CreateMediaProperty>, CreateMediaPropertyValidator>();
#endregion

#region Validation Response Format
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var errors = context.ModelState
            .Where(x => x.Value?.Errors.Count > 0)
            .SelectMany(x => x.Value!.Errors.Select(e => e.ErrorMessage))
            .ToList();

        return new BadRequestObjectResult(BaseResponse.Fail(string.Join(" | ", errors)));
    };
});
#endregion

#region DbContext
builder.Services.AddDbContext<BinaDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
#endregion

#region Identity
builder.Services
    .AddIdentity<User, IdentityRole<int>>(options =>
    {
        options.User.RequireUniqueEmail = true;
        options.Password.RequiredLength = 8;
        options.Password.RequireDigit = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireNonAlphanumeric = true;
        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10);
        options.Lockout.MaxFailedAccessAttempts = 4;
        options.Lockout.AllowedForNewUsers = true;
    })
    .AddEntityFrameworkStores<BinaDbContext>()
    .AddDefaultTokenProviders();
#endregion

#region Repositories
builder.Services.AddScoped(typeof(IRepository<,>), typeof(GenericRepository<,>));
builder.Services.AddScoped<ICityRepository, CityRepository>();
builder.Services.AddScoped<IDistrictRepository, DistrictRepository>();
builder.Services.AddScoped<IPropertyListeningRepository, PropertyListeningRepository>();
builder.Services.AddScoped<IMediaPropertyRepository, MediaRepository>();
builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
#endregion

#region Domain Services
builder.Services.AddScoped<ICityService, CityService>();
builder.Services.AddScoped<IDistrictService, DistrictService>();
builder.Services.AddScoped<IPropertyListingService, PropertyListingService>();
builder.Services.AddScoped<IMediaPropertyService, MediaPropertyService>();
builder.Services.AddScoped<IRecommendationService, RecommendationService>();
#endregion

#region Auth / Token Services
builder.Services.AddScoped<IRefreshTokenService, RefreshTokenService>();
builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
builder.Services.AddScoped<IAuthService, AuthService>();
#endregion

#region MinIO
builder.Services.AddMinioStorage(builder.Configuration);
builder.Services.AddScoped<IFileStorageService, S3MinioFileStorageService>();
#endregion

#region AutoMapper
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
#endregion

#region EmailServices


#endregion

// ✅ Addım 6: JWT Auth + Authorization Policies + Swagger + Options (Jwt/Seed) hamısı burda
builder.Services.AddAppServices(builder.Configuration);

var app = builder.Build();

// ✅ Addım 7: Pipeline + Seed (RoleSeeder/AdminSeeder) hamısı burda
app.ConfigurePipeline();

app.Run();

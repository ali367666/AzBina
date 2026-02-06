using API.Middlewares;
using Application.Abstracts.Repositories;
using Application.Abstracts.Services;
using Application.DTOs.CityDTOs.RequestDTOs;
using Application.DTOs.DistrictDTOs.RequestDTOs;
using Application.DTOs.MediaPropertyDTOs.RequestDTOs;
using Application.DTOs.PropertyListeningDTOs.RequestDTOs;
using Application.Shared.Helpers.Responses;
using Application.Validations.CityValidation;
using Application.Validations.DistrictValidation;
using Application.Validations.MediaPropertyValidation;
using Application.Validations.PropertyListingValidation;
using FluentValidation;
using FluentValidation.AspNetCore;
using Infrastructure.Extensions;
using Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;
using Persistence.Repositories;
using Persistence.Services;
using System.Text.Json.Serialization;
using API.Extensions;


var builder = WebApplication.CreateBuilder(args);

#region Controllers + Validation
builder.Services
    .AddControllers()
    .AddJsonOptions(o =>
        o.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles)
    .AddFluentValidation(fv =>
    {
        fv.RegisterValidatorsFromAssemblyContaining<CreateDistrictValidation>();
        fv.RegisterValidatorsFromAssemblyContaining<CreateCityValidation>();
        fv.RegisterValidatorsFromAssemblyContaining<CreatePropertyListingValidation>();
        fv.RegisterValidatorsFromAssemblyContaining<CreateMediaPropertyValidator>();
    });

// Explicit validators
builder.Services.AddScoped<IValidator<DistrictCreateDTO>, CreateDistrictValidation>();
builder.Services.AddScoped<IValidator<CreateCityDTOs>, CreateCityValidation>();
builder.Services.AddScoped<IValidator<CreatePropertyListing>, CreatePropertyListingValidation>();
builder.Services.AddScoped<IValidator<CreateMediaProperty>, CreateMediaPropertyValidator>();

builder.Services.AddApplicationServices(builder.Configuration);

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

        var message = string.Join(" | ", errors);
        return new BadRequestObjectResult(BaseResponse.Fail(message));
    };
});
#endregion

#region Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
#endregion

#region Database
builder.Services.AddDbContext<BinaDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));
#endregion

#region Repositories
builder.Services.AddScoped(typeof(IRepository<,>), typeof(GenericRepository<,>));

builder.Services.AddScoped<ICityRepository, CityRepository>();
builder.Services.AddScoped<IDistrictRepository, DistrictRepository>();
builder.Services.AddScoped<IPropertyListeningRepository, PropertyListeningRepository>();
builder.Services.AddScoped<IMediaPropertyRepository, MediaRepository>();
#endregion

#region Services
builder.Services.AddScoped<ICityService, CityService>();
builder.Services.AddScoped<IDistrictService, DistrictService>();
builder.Services.AddScoped<IPropertyListingService, PropertyListingService>();
builder.Services.AddScoped<IMediaPropertyService, MediaPropertyService>();
#endregion

/*#region MinIO (IMPORTANT)
builder.Services.AddMinioStorage(builder.Configuration);
builder.Services.AddScoped<IFileStorageService, S3MinioFileStorageService>();
#endregion*/

#region AutoMapper
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
#endregion

#region Authorization
builder.Services.AddAuthorization();
#endregion

var app = builder.Build();

#region Middleware Pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

// Global exception middleware (istəsən aç)
// app.UseMiddleware<ExceptionMiddleware>();

app.UseAuthorization();

app.MapControllers();
#endregion

app.Run();

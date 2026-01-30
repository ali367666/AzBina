using API.Middlewares;
using Application.Abstracts.Repositories;
using Application.Abstracts.Services;
using Application.DTOs.CityDTOs.RequestDTOs;
using Application.DTOs.DistrictDTOs.RequestDTOs;
using Application.Shared.Helpers.Responses;
using Application.Validations.CityValidation;
using Application.Validations.DistrictValidation;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;
using Persistence.Repositories;
using Persistence.Services;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// ✅ MVC mütləq olmalıdır (yoxsa validation da işləməz)
builder.Services
    .AddControllers()
    .AddJsonOptions(o =>
        o.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles
    )
    .AddFluentValidation(fv =>
    {
        fv.RegisterValidatorsFromAssemblyContaining<CreateDistrictValidation>();
        fv.RegisterValidatorsFromAssemblyContaining<CreateCityValidation>();
        // fv.DisableDataAnnotationsValidation = true; // istəsən aça bilərsən
    });

// ✅ (opsional amma qarantili) explicit validator register
builder.Services.AddScoped<IValidator<DistrictCreateDTO>, CreateDistrictValidation>();
builder.Services.AddScoped<IValidator<CreateCityDTOs>, CreateCityValidation>();

// ✅ Validation error-ları BaseResponse formatında
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

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<BinaDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// DI
builder.Services.AddScoped(typeof(IRepository<,>), typeof(GenericRepository<,>));

builder.Services.AddScoped<ICityRepository, CityRepository>();
builder.Services.AddScoped<ICityService, CityService>();

builder.Services.AddScoped<IDistrictRepository, DistrictRepository>();
builder.Services.AddScoped<IDistrictService, DistrictService>();

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// ✅ Authorization istifadə edirsənsə, bu build-dən ƏVVƏL olmalıdır
builder.Services.AddAuthorization();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Global exception middleware (istəsən aç)
app.UseMiddleware<ExceptionMiddleware>();

// Əgər [Authorize] istifadə etmirsənsə, bunu da söndürə bilərsən
app.UseAuthorization();

app.MapControllers();
app.UseMiddleware<ExceptionMiddleware>();

app.Run();

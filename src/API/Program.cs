using Application.Abstracts.Repositories;
using Application.Abstracts.Services;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;
using Persistence.Repositories;
using Persistence.Services;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<BinaDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped(typeof(IRepository<,>), typeof(GenericRepository<,>));

// ICityRepository -> CityRepository
builder.Services.AddScoped<ICityRepository, CityRepository>();

builder.Services.AddScoped<ICityService, CityService>();
builder.Services.AddScoped<IDistrictRepository, DistrictRepository>();
builder.Services.AddScoped<IDistrictService, DistrictService>();



builder.Services.AddControllers()
    .AddJsonOptions(o =>
        o.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles
    );



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

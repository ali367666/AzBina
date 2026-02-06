using Application.Abstracts.Services;
using Infrastructure.Extensions;
using Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace API.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // DbContext və digər servislərdən əvvəl
        services.AddMinioStorage(configuration);

        // IFileStorageService -> MinIO implementation
        services.AddScoped<IFileStorageService, S3MinioFileStorageService>();

        return services;
    }
}

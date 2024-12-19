using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using QuizAI.Application.Interfaces;
using QuizAI.Application.Services;
using QuizAI.Domain.Repositories;

namespace QuizAI.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddApplication(this IServiceCollection services)
    {
        var applicationAssembly = typeof(ServiceCollectionExtensions).Assembly;
        
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(applicationAssembly));
        services.AddAutoMapper(applicationAssembly);
        services.AddValidatorsFromAssembly(applicationAssembly)
            .AddFluentValidationAutoValidation();

        var storageFolderName = "Uploads";

        services.AddScoped<IFileStorageService, FileStorageService>(provider =>
        {
            var environment = provider.GetRequiredService<IHostEnvironment>();
            return new FileStorageService(environment, storageFolderName);
        });

        services.AddScoped<IImageService, ImageService>(provider =>
        {
            var imagesRepository = provider.GetRequiredService<IImagesRepository>();
            var fileStorageService = provider.GetRequiredService<IFileStorageService>();
            (ushort, ushort) imagesDefaultSize = (800, 800);
            var imagesMaxSizeInBytes = 2 * 1024 * 1024; // 2MB
            return new ImageService(imagesRepository, fileStorageService, imagesDefaultSize, imagesMaxSizeInBytes);
        });

        services.AddScoped<ICategoryService, CategoryService>();
    }
}

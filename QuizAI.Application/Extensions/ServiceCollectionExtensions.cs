using AutoMapper;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using QuizAI.Application.Interfaces;
using QuizAI.Application.Services;
using QuizAI.Application.Users;
using QuizAI.Domain.Repositories;

namespace QuizAI.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddApplication(this IServiceCollection services, IConfiguration configuration, string publicStoragePath, string privateStoragePath)
    {
        var applicationAssembly = typeof(ServiceCollectionExtensions).Assembly;
        
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(applicationAssembly));
        services.AddAutoMapper(applicationAssembly);
        services.AddValidatorsFromAssembly(applicationAssembly)
            .AddFluentValidationAutoValidation();

        services.AddHttpContextAccessor();
        services.AddScoped<IUserContext, UserContext>();

        services.AddScoped<IFileStorageService>(provider => new FileStorageService(publicStoragePath, privateStoragePath));

        services.AddScoped<IImageService, ImageService>(provider =>
        {
            var repository = provider.GetRequiredService<IRepository>();
            var fileStorageService = provider.GetRequiredService<IFileStorageService>();
            var imagesRepository = provider.GetRequiredService<IImagesRepository>();
            (ushort, ushort) imagesDefaultSize = (800, 800);
            return new ImageService(repository, fileStorageService, imagesRepository, imagesDefaultSize);
        });

        services.AddScoped<IQuestionService, QuestionService>(provider =>
        {
            var mapper = provider.GetRequiredService<IMapper>();
            var imagesRepository = provider.GetRequiredService<IImagesRepository>();
            byte maxNumberOfQuestions = 20;
            return new QuestionService(mapper, imagesRepository, maxNumberOfQuestions);
        });

        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IQuizService, QuizService>();
        services.AddScoped<IAnswerService, AnswerService>();
        services.AddScoped<IQuizAttemptService, QuizAttemptService>();
        services.AddScoped<IQuizAuthorizationService, QuizAuthorizationService>();

        services.AddScoped<IOpenAiService, OpenAiService>(provider => new OpenAiService(configuration["OpenAI:ApiKey"]!, configuration["OpenAI:Model"]!));
    }
}

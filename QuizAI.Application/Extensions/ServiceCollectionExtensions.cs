using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using QuizAI.Application.Interfaces;
using QuizAI.Application.Services;
using QuizAI.Application.Users;
using QuizAI.Domain.Entities;
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
            var imagesRepository = provider.GetRequiredService<IImagesRepository>();
            byte maxNumberOfQuestions = 20;
            return new QuestionService(imagesRepository, maxNumberOfQuestions);
        });

        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IQuizService, QuizService>();
        services.AddScoped<IAnswerService, AnswerService>();
        services.AddScoped<IQuizAttemptService, QuizAttemptService>();
        services.AddScoped<IQuizAuthorizationService, QuizAuthorizationService>();

        services.AddScoped<IOpenAiService, OpenAiService>(provider => new OpenAiService(configuration["OpenAI:ApiKey"]!, configuration["OpenAI:Model"]!));

        services.AddScoped<IAuthService, AuthService>(provider =>
        {
            var httpContextAccessor = provider.GetService<IHttpContextAccessor>();
            var config = provider.GetRequiredService<IConfiguration>();
            var accessTokenExpirationInMinutes = double.Parse(config["JwtSettings:AccessToken:ExpirationInMinutes"]!);
            var refreshTokenExpirationInMinutes = double.Parse(config["JwtSettings:RefreshToken:ExpirationInMinutes"]!);
            return new AuthService(httpContextAccessor!, config["JwtSettings:Key"]!, accessTokenExpirationInMinutes, refreshTokenExpirationInMinutes);
        });

        services.AddSingleton<IEmailSender, EmailSenderService>(provider =>
        {
            var config = provider.GetRequiredService<IConfiguration>();
            var smtpServer = config["EmailSettings:SmtpServer"]!;
            var fromEmail = config["EmailSettings:FromEmail"]!;
            var password = config["EmailSettings:Password"]!;
            var port = int.Parse(config["EmailSettings:Port"]!);
            return new EmailSenderService(smtpServer, fromEmail, password, port);
        });
        var queueName = "emailQueue";
        services.AddSingleton<IRabbitMqService, RabbitMqService>(provider => new RabbitMqService(provider.GetService<IHttpContextAccessor>()!, queueName));
        services.AddSingleton<IEmailConsumerService, EmailConsumerService>(provider => new EmailConsumerService(provider.GetService<IEmailSender>()!, queueName));
        services.AddHostedService<EmailConsumerHostedService>();
    }
}

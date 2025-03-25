using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using QuizAI.Application.Interfaces;
using QuizAI.Application.Services;
using QuizAI.Application.Users;
using QuizAI.Domain.Exceptions;
using QuizAI.Domain.Interfaces;
using QuizAI.Domain.Repositories;

namespace QuizAI.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        var applicationAssembly = typeof(ServiceCollectionExtensions).Assembly;
        
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(applicationAssembly));
        services.AddAutoMapper(applicationAssembly);
        services.AddValidatorsFromAssembly(applicationAssembly)
            .AddFluentValidationAutoValidation();

        services.AddHttpContextAccessor();
        services.AddScoped<IUserContext, UserContext>();

        services.AddScoped<IImageService, ImageService>(provider =>
        {
            var repository = provider.GetRequiredService<IRepository>();
            var blobStorageService = provider.GetRequiredService<IBlobStorageService>();
            var imagesRepository = provider.GetRequiredService<IImagesRepository>();
            (ushort, ushort) imagesDefaultSize = (800, 800);
            return new ImageService(repository, blobStorageService, imagesRepository, imagesDefaultSize);
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

        services.AddScoped<IOpenAiService, OpenAiService>(
            provider => new OpenAiService(
                Environment.GetEnvironmentVariable("OPENAI_API_KEY") ?? throw new ConflictException("Missing OPENAI_API_KEY"), 
                configuration["OpenAI:Model"]!)
            );

        services.AddScoped<IAuthService, AuthService>(provider =>
        {
            var jwtKey = Environment.GetEnvironmentVariable("JWT_KEY") ?? throw new ConflictException("Missing JWT_KEY"); ;
            var accessTokenExpirationInMinutes = double.Parse(configuration["JwtSettings:AccessToken:ExpirationInMinutes"]!);
            var refreshTokenExpirationInMinutes = double.Parse(configuration["JwtSettings:RefreshToken:ExpirationInMinutes"]!);
            return new AuthService(provider.GetService<IHttpContextAccessor>()!, jwtKey, accessTokenExpirationInMinutes, refreshTokenExpirationInMinutes);
        });

        services.AddSingleton<IEmailSender, EmailSenderService>(provider =>
        {
            var port = int.Parse(configuration["EmailSettings:Port"]!);
            var smtpServer = configuration["EmailSettings:SmtpServer"]!;
            var fromEmail = configuration["EmailSettings:FromEmail"]!;
            var password = Environment.GetEnvironmentVariable("EMAIL_PASSWORD") ?? throw new ConflictException("Missing EMAIL_PASSWORD");
            return new EmailSenderService(smtpServer, fromEmail, password, port);
        });
        services.AddHostedService<EmailConsumerHostedService>();
    }
}

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using QuizAI.Domain.Entities;
using QuizAI.Domain.Exceptions;
using QuizAI.Domain.Interfaces;
using QuizAI.Domain.Repositories;
using QuizAI.Infrastructure.Messaging;
using QuizAI.Infrastructure.Persistence;
using QuizAI.Infrastructure.Repositories;
using QuizAI.Infrastructure.Storage;

namespace QuizAI.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddInfrastructure(this IServiceCollection services)
    {
        services.AddDbContext<AppDbContext>(options => options.UseSqlServer(
            Environment.GetEnvironmentVariable("QUIZAI_DB_CONNECTION_STRING") ?? throw new ConflictException("Missing QUIZAI_DB_CONNECTION_STRING")));

        services.AddIdentity<User, IdentityRole>(options =>
        {
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireUppercase = true;
            options.Password.RequireNonAlphanumeric = true;
            options.Password.RequiredLength = 8;
            options.User.RequireUniqueEmail = true;
        })
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();

        services.AddScoped<IRepository, Repository>();
        services.AddScoped<ICategoriesRepository, CategoriesRepository>();
        services.AddScoped<IImagesRepository, ImagesRepository>();
        services.AddScoped<IQuizzesRepository, QuizzesRepository>();
        services.AddScoped<IQuestionsRepository, QuestionsRepository>();
        services.AddScoped<IQuizAttemptsRepository, QuizAttemptsRepository>();
        services.AddScoped<IAnswersRepository,  AnswersRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IQuizPermissionsRepository, QuizPermissionsRepository>();

        services.AddSingleton<IBlobStorageService, BlobStorageService>(provider =>
        {
            var connectionString = Environment.GetEnvironmentVariable("AZURE_BLOB_CONNECTION_STRING") 
                ?? throw new ConflictException("Missing AZURE_BLOB_CONNECTION_STRING");
            var publicContainerName = Environment.GetEnvironmentVariable("AZURE_BLOB_PUBLIC_CONTAINER_NAME")
                ?? throw new ConflictException("Missing AZURE_BLOB_PUBLIC_CONTAINER_NAME");
            var privateContainerName = Environment.GetEnvironmentVariable("AZURE_BLOB_PRIVATE_CONTAINER_NAME")
                ?? throw new ConflictException("Missing AZURE_BLOB_PRIVATE_CONTAINER_NAME");

            return new BlobStorageService(connectionString, publicContainerName, privateContainerName);
        });

        var rabbitMqQueueName = "emailQueue";
        services.AddSingleton<IRabbitMqService, RabbitMqService>(provider =>
        {
            var hostName = Environment.GetEnvironmentVariable("RABBITMQ_HOST_NAME") ?? throw new ConflictException("Missing RABBITMQ_HOST_NAME");
            var port = int.Parse(Environment.GetEnvironmentVariable("RABBITMQ_PORT") ?? throw new ConflictException("Missing RABBITMQ_PORT"));
            return new RabbitMqService(provider.GetService<IHttpContextAccessor>()!, rabbitMqQueueName, hostName, port);
        });
        services.AddSingleton<IEmailConsumerService, EmailConsumerService>(provider =>
        {
            var hostName = Environment.GetEnvironmentVariable("RABBITMQ_HOST_NAME") ?? throw new ConflictException("Missing RABBITMQ_HOST_NAME");
            var port = int.Parse(Environment.GetEnvironmentVariable("RABBITMQ_PORT") ?? throw new ConflictException("Missing RABBITMQ_PORT"));
            return new EmailConsumerService(provider.GetService<IEmailSender>()!, rabbitMqQueueName, hostName, port);
        });
    }
}

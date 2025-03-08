using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using QuizAI.Domain.Entities;
using QuizAI.Domain.Repositories;
using QuizAI.Infrastructure.Persistence;
using QuizAI.Infrastructure.Repositories;

namespace QuizAI.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("QuizAIDatabase");
        services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));

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
    }
}

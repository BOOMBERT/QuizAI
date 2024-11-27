using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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

        services.AddScoped<IRepository, Repository>();
        services.AddScoped<ICategoriesRepository, CategoriesRepository>();
        services.AddScoped<IImagesRepository, ImagesRepository>();
    }
}

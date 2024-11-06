using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using QuizAI.Infrastructure.Persistence;

namespace QuizAI.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("QuizAIDatabase");
        services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));
    }
}

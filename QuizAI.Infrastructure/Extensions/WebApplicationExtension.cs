using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using QuizAI.Infrastructure.Persistence;

namespace QuizAI.Infrastructure.Extensions;

public static class WebApplicationExtension
{
    public static async void ApplyMigrations(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            if (dbContext.Database.GetPendingMigrations().Any())
                await dbContext.Database.MigrateAsync();
        }
    }
}

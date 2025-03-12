using Microsoft.Extensions.DependencyInjection;
using QuizAI.Domain.Entities;
using QuizAI.Infrastructure.Persistence;

namespace QuizAI.API.Tests;

internal class TestDatabaseInitializer
{
    private readonly IServiceProvider _serviceProvider;

    internal TestDatabaseInitializer(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task ConfigureDatabaseAndAddAuthUserAsync(string userId = "1", bool emailConfirmed = true)
    {
        var user = new User()
        {
            Id = userId,
            Email = "test@test.com",
            UserName = "test@test.com",
            NormalizedEmail = "TEST@TEST.COM",
            EmailConfirmed = emailConfirmed
        };

        using (var scope = _serviceProvider.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            db.Database.EnsureCreated();

            await db.AddAsync(user);
            await db.SaveChangesAsync();
        }
    }
}

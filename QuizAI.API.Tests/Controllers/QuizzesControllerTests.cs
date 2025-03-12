using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using QuizAI.API.Tests;
using QuizAI.Infrastructure.Persistence;
using System.Net;
using Xunit;

namespace QuizAI.API.Controllers.Tests;

public class QuizzesControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly CustomWebApplicationFactory<Program> _factory;
    private readonly TestDatabaseInitializer _dbInitializer;

    public QuizzesControllerTests(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _dbInitializer = new TestDatabaseInitializer(_factory.Services);
    }

    [Fact]
    public async Task CreateQuiz_WhenValidRequest_ShouldReturn201Created()
    {
        // Arrange

        await _dbInitializer.ConfigureDatabaseAndAddAuthUserAsync();

        var client = _factory.CreateClient();

        var formData = new MultipartFormDataContent
        {
            { new StringContent("Test quiz name"), "Name" },
            { new StringContent("Test quiz description"), "Description" },
            { new StringContent("true"), "IsPrivate" },
            { new StringContent("Test category 1"), "Categories" },
            { new StringContent("Test category 2"), "Categories" },
        };

        // Act

        var result = await client.PostAsync("/api/quizzes", formData);

        // Assert

        Assert.Equal(HttpStatusCode.Created, result.StatusCode);

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var quiz = await db.Quizzes
            .Include(q => q.Categories)
            .FirstOrDefaultAsync();

        Assert.NotNull(quiz);
        Assert.Equal("Test quiz name", quiz.Name);
        Assert.Equal("Test quiz description", quiz.Description);
        Assert.True(quiz.IsPrivate);
        Assert.Equal(2, quiz.Categories.Count);
        Assert.NotEqual((DateTime?)null, quiz.CreationDate);
        Assert.False(quiz.IsDeprecated);
        Assert.Equal("1", quiz.CreatorId);
        Assert.Equal(0, quiz.QuestionCount);

        Assert.Equal($"http://localhost/api/quizzes/{quiz.Id}", result.Headers.Location?.ToString());
    }
}

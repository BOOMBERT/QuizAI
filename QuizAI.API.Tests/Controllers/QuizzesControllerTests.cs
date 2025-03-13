using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using QuizAI.API.Tests;
using QuizAI.Application.Common;
using QuizAI.Application.Quizzes.Commands.UpdateQuiz;
using QuizAI.Application.Quizzes.Dtos;
using QuizAI.Domain.Entities;
using QuizAI.Infrastructure.Persistence;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Xunit;

namespace QuizAI.API.Controllers.Tests;

public class QuizzesControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly CustomWebApplicationFactory<Program> _factory;
    private readonly TestDatabaseInitializer _dbInitializer;
    private const string BASE_ROUTE_PATH = "api/quizzes";

    public QuizzesControllerTests(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _dbInitializer = new TestDatabaseInitializer(_factory.Services);
    }

    [Fact]
    public async Task CreateQuiz_WhenValidRequest_ShouldCreateQuizAndReturn201Created()
    {
        // Arrange

        await _dbInitializer.ConfigureDatabaseAndAddAuthUserAsync();

        var client = _factory.CreateClient();

        var quizWithTheSameCategory = new Quiz()
        {
            Id = Guid.NewGuid(),
            Name = "Test quiz name 2",
            CreatorId = "1",
            Categories = new List<Category>() { new() { Id = 2, Name = "test category 2" } }
        };

        var formData = new MultipartFormDataContent
        {
            { new StringContent("Test quiz name"), "Name" },
            { new StringContent("Test quiz description"), "Description" },
            { new StringContent("true"), "IsPrivate" },
            { new StringContent("test category 1"), "Categories" },
            { new StringContent("test category 2"), "Categories" },
        };

        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            
            await db.AddAsync(quizWithTheSameCategory);
            await db.SaveChangesAsync();
        }

        // Act

        var result = await client.PostAsync(BASE_ROUTE_PATH, formData);

        // Assert

        Assert.Equal(HttpStatusCode.Created, result.StatusCode);

        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var quiz = await db.Quizzes
                .Include(qz => qz.Categories)
                .FirstOrDefaultAsync(qz => qz.Id != quizWithTheSameCategory.Id);
            
            Assert.NotNull(quiz);
            Assert.Equal("Test quiz name", quiz.Name);
            Assert.Equal("Test quiz description", quiz.Description);
            Assert.True(quiz.IsPrivate);
            Assert.Equal(2, quiz.Categories.Count);
            Assert.NotEqual((DateTime?)null, quiz.CreationDate);
            Assert.False(quiz.IsDeprecated);
            Assert.Equal("1", quiz.CreatorId);
            Assert.Equal(0, quiz.QuestionCount);
            Assert.Null(quiz.LatestVersionId);

            Assert.Equal(2, await db.Categories.CountAsync());

            Assert.Equal($"http://localhost/api/quizzes/{quiz.Id}", result.Headers.Location?.ToString());
        }
    }

    [Fact]
    public async Task GetQuizById_WhenValidRequest_ShouldReturnQuizDtoWith200Ok()
    {
        // Arrange

        await _dbInitializer.ConfigureDatabaseAndAddAuthUserAsync();

        var client = _factory.CreateClient();

        var (quizId, imageId) = (Guid.NewGuid(), Guid.NewGuid());
        var quiz = new Quiz()
        {
            Id = quizId,
            Name = "Test name",
            Description = "Test Description",
            Categories = new List<Category>() { new() { Name = "test category 1" }, new() { Name = "test category 2" } },
            CreatorId = "1",
            ImageId = imageId,
            Image = new Image() { Id = imageId, FileExtension = ".png", Hash = new byte[] { 1, 2, 3 } },
            QuizAttempts = new List<QuizAttempt>() { new() { QuizId = quizId, UserId = "1" } },
            QuestionCount = 10
        };

        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            await db.AddAsync(quiz);
            await db.SaveChangesAsync();
        }

        // Act

        var result = await client.GetAsync($"{BASE_ROUTE_PATH}/{quiz.Id}");
        var quizDto = await result.Content.ReadFromJsonAsync<QuizDto>();

        // Assert

        Assert.Equal(HttpStatusCode.OK, result.StatusCode);

        Assert.NotNull(quizDto);
        Assert.Equal(quiz.Name, quizDto.Name);
        Assert.Equal(quiz.Description, quizDto.Description);
        Assert.Equal(quiz.Categories.Select(c => c.Name), quizDto.Categories);
        Assert.Equal(quiz.CreatorId, quizDto.CreatorId);
        Assert.Equal(quiz.IsDeprecated, quizDto.IsDeprecated);
        Assert.Equal(quiz.IsPrivate, quizDto.IsPrivate);
        Assert.Equal(quiz.LatestVersionId, quizDto.LatestVersionId);
        Assert.Equal(quiz.QuestionCount, quizDto.QuestionCount);
        Assert.NotEqual((DateTime?)null, quiz.CreationDate);
        
        Assert.True(quizDto.HasImage);
        Assert.True(quizDto.CanEdit);
        Assert.True(quizDto.HasUnfinishedAttempt);
        Assert.Equal($"/api/uploads/{imageId}.png", quizDto.PublicImageUrl);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task UpdateQuiz_WhenValidRequestAndQuizAttemptsExistOrNot_ShouldUpdateOrCreateAndReturnLatestQuizIdWith200Ok(bool withQuizAttempts)
    {
        // Arrange

        await _dbInitializer.ConfigureDatabaseAndAddAuthUserAsync();

        var client = _factory.CreateClient();

        var (quizId, imageId) = (Guid.NewGuid(), Guid.NewGuid());
        var quiz = new Quiz()
        {
            Id = quizId,
            Name = "Test quiz name",
            Description = "Test quiz Description",
            Categories = new List<Category>() { new() { Name = "test category 1" } },
            QuizAttempts = withQuizAttempts ? new List<QuizAttempt>() { new() { QuizId = quizId, UserId = "1" } } : new List<QuizAttempt>(),
            ImageId = imageId,
            Image = new Image() { Id = imageId, FileExtension = ".png", Hash = new byte[] { 1, 2, 3 } },
            CreatorId = "1",
        };

        var quizWithTheSameCategoryAndNewCategory = new Quiz()
        {
            Id = Guid.NewGuid(),
            Name = "Test quiz name 2",
            CreatorId = "1",
            Categories = new List<Category>() { new() { Id = 4, Name = "test category 4"} }
        };

        var commonCategory = new Category() { Id = 2, Name = "test category 2" };
        quiz.Categories.Add(commonCategory);
        quizWithTheSameCategoryAndNewCategory.Categories.Add(commonCategory);

        var deprecatedVersionOfQuiz = new Quiz()
        {
            Id = Guid.NewGuid(),
            Name = "Deprecated quiz",
            IsDeprecated = true,
            LatestVersionId = quizId,
            CreatorId = quiz.CreatorId
        };

        var users = new List<User>()
        {
            new() { 
                Id = "2", 
                QuizPermissions = new List<QuizPermission>() 
                { 
                    new() { QuizId = quizId, UserId = "2", CanEdit = true, CanPlay = true } 
                }, 
                QuizAttempts = withQuizAttempts ? new List<QuizAttempt>() 
                { 
                    new() { QuizId = quizId, UserId = "2", FinishedAt = DateTime.UtcNow } 
                } : new List<QuizAttempt>() },
            new() { 
                Id = "3", 
                QuizPermissions = new List<QuizPermission>() 
                {
                    new() { QuizId = quizId, UserId = "3", CanEdit = true, CanPlay = true } 
                }, 
                QuizAttempts = withQuizAttempts ? new List<QuizAttempt>() 
                { 
                    new() { QuizId = quizId, UserId = "3", FinishedAt = null } 
                } : new List<QuizAttempt>() }
        };

        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            await db.AddAsync(quiz);
            await db.AddAsync(quizWithTheSameCategoryAndNewCategory);
            await db.AddAsync(deprecatedVersionOfQuiz);
            await db.AddRangeAsync(users);
            await db.SaveChangesAsync();
        }

        var command = new UpdateQuizCommand()
        {
            Name = "Updated test quiz name",
            Description = "Updated test quiz description",
            Categories = new List<string>() { "Test category 1", "TEST category 3", "test category 4" },
        };

        var content = new StringContent(JsonSerializer.Serialize(command), Encoding.UTF8, "application/json");

        // Act

        var result = await client.PutAsync($"{BASE_ROUTE_PATH}/{quiz.Id}", content);
        var latestQuizId = await result.Content.ReadFromJsonAsync<LatestQuizId>();

        // Assert

        Assert.Equal(HttpStatusCode.OK, result.StatusCode);

        Assert.NotNull(latestQuizId);

        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var baseQuizToUpdate = await db.Quizzes
                .Include(qz => qz.Categories)
                .Include(qz => qz.QuizAttempts)
                .Include(qz => qz.QuizPermissions)
                .FirstOrDefaultAsync(qz => qz.Id == quizId);

            Assert.NotNull(baseQuizToUpdate);

            if (withQuizAttempts)
            {
                var newUpdatedQuiz = await db.Quizzes
                    .Include(qz => qz.Categories)
                    .Include(qz => qz.QuizAttempts)
                    .Include(qz => qz.QuizPermissions)
                    .FirstOrDefaultAsync(qz => qz.Name == command.Name);

                Assert.NotNull(newUpdatedQuiz);
                Assert.Equal(newUpdatedQuiz.Id, latestQuizId.latestQuizId);

                Assert.Equal(quiz.Id, baseQuizToUpdate.Id);
                Assert.Equal(quiz.Name, baseQuizToUpdate.Name);
                Assert.Equal(quiz.Description, baseQuizToUpdate.Description);
                Assert.Empty(baseQuizToUpdate.Categories);
                Assert.Null(baseQuizToUpdate.ImageId);

                Assert.Equal(quiz.QuizAttempts.Count, baseQuizToUpdate.QuizAttempts.Count);
                Assert.True(baseQuizToUpdate.IsDeprecated);
                Assert.Equal(newUpdatedQuiz.Id, baseQuizToUpdate.LatestVersionId);

                Assert.Equal(command.Name, newUpdatedQuiz.Name);
                Assert.Equal(command.Description, newUpdatedQuiz.Description);
                Assert.Equal(command.Categories.Select(c => c.ToLower()), newUpdatedQuiz.Categories.Select(c => c.Name).OrderBy(c => c));
                Assert.Equal(quiz.ImageId, newUpdatedQuiz.ImageId);
                Assert.Empty(newUpdatedQuiz.QuizAttempts);

                Assert.Single(baseQuizToUpdate.QuizPermissions);
                Assert.Equal("3", baseQuizToUpdate.QuizPermissions.Select(qp => qp.UserId).FirstOrDefault());
                Assert.Equal(users.Count, newUpdatedQuiz.QuizPermissions.Count);

                Assert.Equal(newUpdatedQuiz.Id, await db.Quizzes
                    .Where(qz => qz.Id == deprecatedVersionOfQuiz.Id)
                    .Select(qz => qz.LatestVersionId)
                    .FirstOrDefaultAsync()
                );
            }
            else
            {
                Assert.Equal(quiz.Id, latestQuizId.latestQuizId);

                Assert.Equal(command.Name, baseQuizToUpdate.Name);
                Assert.Equal(command.Description, baseQuizToUpdate.Description);
                Assert.Equal(command.Categories.Select(c => c.ToLower()), baseQuizToUpdate.Categories.Select(c => c.Name).OrderBy(c => c));

                Assert.Equal(users.Count, baseQuizToUpdate.QuizPermissions.Count);
            }

            Assert.Equal(command.Categories.Concat(
                quizWithTheSameCategoryAndNewCategory.Categories.Select(c => c.Name)).Distinct().Count(), 
                await db.Categories.CountAsync()
            );
            Assert.Equal(quizWithTheSameCategoryAndNewCategory.Categories.Count, await db.Quizzes
                .Include(qz => qz.Categories)
                .Where(qz => qz.Id == quizWithTheSameCategoryAndNewCategory.Id)
                .SelectMany(qz => qz.Categories)
                .CountAsync()
            );
        }
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task DeleteQuiz_WhenValidRequestAndQuizAttemptsExistOrNot_ShouldDeleteOrDeprecateQuizAndReturn204NoContent(bool withQuizAttempts)
    {
        // Arrange

        await _dbInitializer.ConfigureDatabaseAndAddAuthUserAsync();

        var client = _factory.CreateClient();

        var quizId = Guid.NewGuid();
        var quiz = new Quiz()
        {
            Id = quizId,
            Name = "Test quiz name",
            Description = "Test quiz description",
            Categories = new List<Category>() { new() { Name = "test category 1" } },
            QuizAttempts = withQuizAttempts ? new List<QuizAttempt>() { new() { QuizId = quizId, UserId = "1" } } : new List<QuizAttempt>(),
            CreatorId = "1",
        };

        var quizWithTheSameCategory = new Quiz()
        {
            Id = Guid.NewGuid(),
            Name = "Test quiz name 2",
            CreatorId = "1"
        };

        var commonCategory = new Category() { Id = 2, Name = "test category 2" };
        quiz.Categories.Add(commonCategory);
        quizWithTheSameCategory.Categories.Add(commonCategory);

        var deprecatedVersionOfQuiz = new Quiz()
        {
            Id = Guid.NewGuid(),
            Name = "Deprecated quiz",
            IsDeprecated = true,
            LatestVersionId = quizId,
            CreatorId = quiz.CreatorId
        };

        var userWithQuizPermssion = new User()
        {
            Id = "2",
            QuizPermissions = new List<QuizPermission>() { new() { QuizId = quizId, UserId = "2", CanEdit = true } }
        };

        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            await db.AddAsync(quiz);
            await db.AddAsync(quizWithTheSameCategory);
            await db.AddAsync(deprecatedVersionOfQuiz);
            await db.AddAsync(userWithQuizPermssion);
            await db.SaveChangesAsync();
        }

        // Act

        var result = await client.DeleteAsync($"{BASE_ROUTE_PATH}/{quiz.Id}");

        // Assert

        Assert.Equal(HttpStatusCode.NoContent, result.StatusCode);

        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            if (withQuizAttempts)
            {
                var deprecatedQuiz = await db.Quizzes
                    .Include(qz => qz.Categories)
                    .Include(qz => qz.QuizPermissions)
                    .FirstOrDefaultAsync(qz => qz.Id == quiz.Id);

                Assert.NotNull(deprecatedQuiz);

                Assert.True(deprecatedQuiz.IsDeprecated);
                Assert.Empty(deprecatedQuiz.Categories);
                Assert.Empty(deprecatedQuiz.QuizPermissions);
            }
            else
            {
                Assert.Null(await db.Quizzes.FirstOrDefaultAsync(qz => qz.Id == quiz.Id));
                Assert.Null(await db.Quizzes
                    .Where(qz => qz.Id == deprecatedVersionOfQuiz.Id)
                    .Select(qz => qz.LatestVersionId)
                    .FirstOrDefaultAsync()
                );
                Assert.Empty(await db.QuizPermissions.ToArrayAsync());
            }

            Assert.Equal(quizWithTheSameCategory.Categories.Count, await db.Quizzes
                .Include(qz => qz.Categories)
                .Where(qz => qz.Id == quizWithTheSameCategory.Id)
                .Select(qz => qz.Categories)
                .CountAsync());
        }
    }
}

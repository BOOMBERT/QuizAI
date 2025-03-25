using AutoMapper;
using Moq;
using QuizAI.Application.Interfaces;
using QuizAI.Application.Quizzes.Dtos;
using QuizAI.Application.Users;
using QuizAI.Domain.Entities;
using QuizAI.Domain.Repositories;
using Xunit;

namespace QuizAI.Application.Quizzes.Queries.GetAllQuizzes.Tests;

public class GetAllQuizzesQueryHandlerTests
{
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly Mock<IUserContext> _userContextMock = new();
    private readonly Mock<IQuizzesRepository> _quizzesRepositoryMock = new();

    private readonly GetAllQuizzesQueryHandler _handler;

    public GetAllQuizzesQueryHandlerTests()
    {
        _handler = new GetAllQuizzesQueryHandler(_mapperMock.Object, _userContextMock.Object, _quizzesRepositoryMock.Object);
    }

    private void SetupAllMocks(GetAllQuizzesQuery query, IEnumerable<Quiz> quizzes)
    {
        var currentUser = new CurrentUser("user-id", "test@test.com");
        _userContextMock.Setup(u => u.GetCurrentUser())
            .Returns(currentUser);

        _quizzesRepositoryMock.Setup(r => r.GetAllMatchingAsync(
            currentUser.Id, query.SearchPhrase, query.PageSize, query.PageNumber, query.SortBy, query.SortDirection,
            query.FilterByCreator, query.FilterByCategories, query.FilterBySharedQuizzes, query.FilterByUnfinishedAttempts))
            .ReturnsAsync((quizzes, default));

        var quizDtos = quizzes.Select(qz => new QuizDto(
            qz.Id, qz.Name, qz.Description, qz.CreationDate, qz.ImageId != null, Enumerable.Empty<string>(), qz.IsPrivate,
            qz.IsDeprecated, qz.LatestVersionId, qz.QuestionCount, qz.CreatorId, default, default, default))
            .ToList();

        _mapperMock.Setup(m => m.Map<IEnumerable<QuizDto>>(quizzes))
            .Returns(quizDtos);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task Handle_WhenValidQuery_ShouldProperlyAssignCanEdit(bool filterByCreator)
    {
        // Arrange

        var query = new GetAllQuizzesQuery() { FilterByCreator = filterByCreator };

        var quizzes = new List<Quiz>()
        {
            new(),
            new() { IsDeprecated = true },
            new() { IsDeprecated = false, CreatorId = "user-id" },
            new() { IsDeprecated = false, CreatorId = "another-user-id" },
            new() { IsDeprecated = false, CreatorId = "another-user-id", QuizPermissions = new List<QuizPermission>() { new QuizPermission() { UserId = "user-id", CanEdit = true } } },
            new() { IsDeprecated = false, CreatorId = "another-user-id", QuizPermissions = new List<QuizPermission>() { new QuizPermission() { UserId = "user-id", CanEdit = false } } },
            new() { IsDeprecated = false, CreatorId = "another-user-id", QuizPermissions = new List<QuizPermission>() { new QuizPermission() { UserId = "another-user-id2", CanEdit = true } } }
        };

        SetupAllMocks(query, quizzes);

        var correctQuizDtosCanEditAssignments = new List<bool>() 
        { 
            filterByCreator, false, true, filterByCreator, true, filterByCreator, filterByCreator 
        };

        // Act

        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert

        var resultQuizDtos = result.Data.ToList();

        for (int i = 0; i < resultQuizDtos.Count; i++)
        {
            var resultQuizDto = resultQuizDtos[i];
            var quizDtoCorrectCanEditAssignment = correctQuizDtosCanEditAssignments[i];

            Assert.Equal(quizDtoCorrectCanEditAssignment, resultQuizDto.CanEdit);
        }
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task Handle_WhenValidQuery_ShouldProperlyAssignHasUnfinishedAttempt(bool filterByUnfinishedAttempts)
    {
        // Arrange

        var query = new GetAllQuizzesQuery() { FilterByUnfinishedAttempts = filterByUnfinishedAttempts };

        var quizzes = new List<Quiz>()
        {
            new(),
            new() { QuizAttempts = new List<QuizAttempt>() { new QuizAttempt() { UserId = "user-id", FinishedAt = null } } },
            new() { QuizAttempts = new List<QuizAttempt>() { new QuizAttempt() { UserId = "user-id", FinishedAt = DateTime.UtcNow.AddMinutes(-10) } } },
            new() { QuizAttempts = new List<QuizAttempt>() { new QuizAttempt() { UserId = "another-user-id", FinishedAt = null } } }
        };

        SetupAllMocks(query, quizzes);

        var correctQuizDtosHasUnfinishedAttemptAssignments = new List<bool>() 
        { 
            filterByUnfinishedAttempts, true, filterByUnfinishedAttempts, filterByUnfinishedAttempts 
        };
        
        // Act

        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert

        var resultQuizDtos = result.Data.ToList();

        for (int i = 0; i < resultQuizDtos.Count; i++)
        {
            var resultQuizDto = resultQuizDtos[i];
            var quizDtoCorrectHasUnfinishedAttemptAssignment = correctQuizDtosHasUnfinishedAttemptAssignments[i];

            Assert.Equal(quizDtoCorrectHasUnfinishedAttemptAssignment, resultQuizDto.HasUnfinishedAttempt);
        }
    }

    [Fact]
    public async Task Handle_WhenValidQuery_ShouldProperlyAssignPublicImageUrl()
    {
        // Arrange

        var query = new GetAllQuizzesQuery();

        var (testImageId, testFileExtension) = (Guid.NewGuid(), ".png");
        var quizzes = new List<Quiz>()
        {
            new(),
            new() { IsPrivate = true },
            new() { IsPrivate = true, ImageId = testImageId, Image = new Image { FileExtension = testFileExtension } },
            new() { IsPrivate = false, ImageId = testImageId, Image = new Image { FileExtension = testFileExtension } },
        };

        SetupAllMocks(query, quizzes);

        var correctQuizDtosPublicImageUrlAssignments = new List<string?>() 
        {
            null, null, null, $"https://quizaistorage.blob.core.windows.net/public-uploads/{testImageId}{testFileExtension}" 
        };

        // Act

        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert

        var resultQuizDtos = result.Data.ToList();

        for (int i = 0; i < resultQuizDtos.Count; i++)
        {
            var resultQuizDto = resultQuizDtos[i];
            var quizDtoCorrectPublicImageUrlAssignment = correctQuizDtosPublicImageUrlAssignments[i];

            Assert.Equal(quizDtoCorrectPublicImageUrlAssignment, resultQuizDto.PublicImageUrl);
        }
    }
}

using Moq;
using QuizAI.Application.Interfaces;
using QuizAI.Application.Users;
using QuizAI.Domain.Exceptions;
using QuizAI.Domain.Repositories;
using Xunit;

namespace QuizAI.Application.Quizzes.Queries.GetQuizStats.Tests;

public class GetQuizStatsQueryHandlerTests
{
    private readonly Mock<IUserContext> _userContextMock;
    private readonly Mock<IQuizzesRepository> _quizzesRepository;
    private readonly Mock<IQuizAttemptsRepository> _quizAttemptsRepository;

    private readonly GetQuizStatsQueryHandler _handler;
    
    public GetQuizStatsQueryHandlerTests()
    {
        _userContextMock = new Mock<IUserContext>();
        _quizzesRepository = new Mock<IQuizzesRepository>();
        _quizAttemptsRepository = new Mock<IQuizAttemptsRepository>();
    
        _handler = new GetQuizStatsQueryHandler(_userContextMock.Object, _quizzesRepository.Object, _quizAttemptsRepository.Object);
    }

    [Fact]
    public async Task Handle_WhenValidQuery_ShouldReturnQuizStatsDto()
    {
        // Arrange

        var query = new GetQuizStatsQuery(Guid.NewGuid(), It.IsAny<bool>());

        var currentUser = new CurrentUser("user-id", "test@test.com");
        _userContextMock.Setup(u => u.GetCurrentUser())
            .Returns(currentUser);

        _quizzesRepository.Setup(r => r.GetCreatorIdAndIsDeprecatedAndLatestVersionIdAsync(query.QuizId))
            .ReturnsAsync((currentUser.Id, false, default));

        var (quizAttemptsCount, averageCorrectAnswers, averageTimeSpent) = (10, 0.2, TimeSpan.FromSeconds(10));
        _quizAttemptsRepository.Setup(r => r.GetDetailedStatsAsync(query.QuizId, It.IsAny<bool>()))
            .ReturnsAsync((quizAttemptsCount, averageCorrectAnswers, averageTimeSpent));

        // Act

        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert

        Assert.Equal(quizAttemptsCount, result.QuizAttemptsCount);
        Assert.Equal(averageCorrectAnswers, result.AverageCorrectAnswers);
        Assert.Equal(averageTimeSpent, result.AverageTimeSpent);
    }

    [Fact]
    public async Task Handle_WhenNonExistingQuiz_ShouldThrowNotFoundException()
    {
        // Arrange

        var query = new GetQuizStatsQuery(Guid.NewGuid(), It.IsAny<bool>());

        _quizzesRepository.Setup(r => r.GetCreatorIdAndIsDeprecatedAndLatestVersionIdAsync(query.QuizId))
            .ReturnsAsync(default((string, bool, Guid?)?));

        // Act

        Func<Task> action = async () => await _handler.Handle(query, CancellationToken.None);

        // Assert

        var exception = await Assert.ThrowsAsync<NotFoundException>(action);
        Assert.Equal($"Quiz with ID {query.QuizId} was not found", exception.Message);
    }

    [Fact]
    public async Task Handle_WhenCurrentUserIsNotQuizCreator_ShouldThrowForbiddenException()
    {
        // Arrange

        var query = new GetQuizStatsQuery(Guid.NewGuid(), It.IsAny<bool>());

        var currentUser = new CurrentUser("user-id", "test@test.com");
        _userContextMock.Setup(u => u.GetCurrentUser())
            .Returns(currentUser);

        _quizzesRepository.Setup(r => r.GetCreatorIdAndIsDeprecatedAndLatestVersionIdAsync(query.QuizId))
            .ReturnsAsync(("creator-id", default, default));

        // Act

        Func<Task> action = async () => await _handler.Handle(query, CancellationToken.None);

        // Assert

        var exception = await Assert.ThrowsAsync<ForbiddenException>(action);
        Assert.Equal($"You do not have permission to view the stats of the quiz with ID {query.QuizId} because you are not its creator", exception.Message);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task Handle_WhenQuizIsDeprecated_ShouldThrowNotFoundQuizWithVersioningException(bool withLatestVersionId)
    {
        // Arrange

        var query = new GetQuizStatsQuery(Guid.NewGuid(), It.IsAny<bool>());

        var currentUser = new CurrentUser("user-id", "test@test.com");
        _userContextMock.Setup(u => u.GetCurrentUser())
            .Returns(currentUser);

        var latestVersionId = withLatestVersionId ? (Guid?)Guid.NewGuid() : null;
        _quizzesRepository.Setup(r => r.GetCreatorIdAndIsDeprecatedAndLatestVersionIdAsync(query.QuizId))
            .ReturnsAsync((currentUser.Id, true, latestVersionId));

        // Act

        Func<Task> action = async () => await _handler.Handle(query, CancellationToken.None);

        // Assert

        var exception = await Assert.ThrowsAsync<NotFoundQuizWithVersioningException>(action);
        Assert.Equal($"The quiz with ID {query.QuizId} was not found - " + 
            (withLatestVersionId
            ? $"The latest version ID is {latestVersionId}"
            : "No latest version is available"),
            exception.Message);
    }
}

using AutoMapper;
using Moq;
using QuizAI.Application.Interfaces;
using QuizAI.Application.Quizzes.Dtos;
using QuizAI.Application.Quizzes.Queries.GetQuizById;
using QuizAI.Application.Users;
using QuizAI.Domain.Entities;
using QuizAI.Domain.Exceptions;
using QuizAI.Domain.Repositories;
using Xunit;

namespace QuizAI.Application.Tests.Quizzes.Queries.GetQuizById;

public class GetQuizByIdQueryHandlerTests
{
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly Mock<IUserContext> _userContextMock = new();
    private readonly Mock<IQuizzesRepository> _quizzesRepositoryMock = new();
    private readonly Mock<IQuizAuthorizationService> _quizAuthorizationServiceMock = new();
    private readonly Mock<IQuizAttemptsRepository> _quizAttemptsRepositoryMock = new();
    private readonly Mock<IImagesRepository> _imagesRepositoryMock = new();

    private readonly GetQuizByIdQueryHandler _handler;

    public GetQuizByIdQueryHandlerTests()
    {
        _handler = new GetQuizByIdQueryHandler(
            _mapperMock.Object, _userContextMock.Object, _quizAuthorizationServiceMock.Object, _quizzesRepositoryMock.Object,
            _quizAttemptsRepositoryMock.Object, _imagesRepositoryMock.Object);
    }

    [Theory]
    [InlineData(false, false, false, false, false)]
    [InlineData(true, true, true, true, true)]
    [InlineData(true, true, true, true, false)]
    [InlineData(true, true, false, true, true)]
    public async Task Handle_WhenValidQuery_ShouldReturnQuizDto(bool canEdit, bool hasUnfinishedAttempt, bool hasImage, bool isDeprecated, bool isPrivate)
    {
        // Arrange

        var query = new GetQuizByIdQuery(Guid.NewGuid());

        var quiz = new Quiz()
        {
            Id = query.QuizId,
            IsPrivate = isPrivate,
            IsDeprecated = isDeprecated,
            ImageId = hasImage ? Guid.NewGuid() : null,
        };

        var currentUser = new CurrentUser("user-id", "test@test.com");
        _userContextMock.Setup(u => u.GetCurrentUser())
            .Returns(currentUser);

        _quizzesRepositoryMock.Setup(r => r.GetAsync(query.QuizId, true, false, false))
            .ReturnsAsync(quiz);

        _quizAuthorizationServiceMock.Setup(s => s.AuthorizeReadOperationAndGetCanEditAsync(quiz, currentUser.Id))
            .ReturnsAsync(canEdit);

        _quizAttemptsRepositoryMock.Setup(r => r.HasAnyAsync(quiz.Id, currentUser.Id, false))
            .ReturnsAsync(hasUnfinishedAttempt);

        var fileExtension = ".png";
        _imagesRepositoryMock.Setup(r => r.GetFileExtensionAsync(It.IsAny<Guid>()))
            .ReturnsAsync(fileExtension);

        var quizDto = new QuizDto(
            quiz.Id, quiz.Name, quiz.Description, quiz.CreationDate, default, Enumerable.Empty<string>(), quiz.IsPrivate, 
            quiz.IsDeprecated, quiz.LatestVersionId, quiz.QuestionCount, quiz.CreatorId, default, default, default);
        _mapperMock.Setup(m => m.Map<QuizDto>(quiz))
            .Returns(quizDto);

        // Act

        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert

        Assert.Equal((!quiz.IsPrivate && quiz.ImageId != null) ? $"https://quizaistorage.blob.core.windows.net/public-uploads/{quiz.ImageId}{fileExtension}" : null, result.PublicImageUrl);
        Assert.Equal(isDeprecated ? false : canEdit, result.CanEdit);
        Assert.Equal(hasUnfinishedAttempt, result.HasUnfinishedAttempt);

        if (!quiz.IsPrivate && quiz.ImageId != null)
        {
            _imagesRepositoryMock.Verify(r => r.GetFileExtensionAsync((Guid)quiz.ImageId), Times.Once);
        }
        else
        {
            _imagesRepositoryMock.Verify(r => r.GetFileExtensionAsync(It.IsAny<Guid>()), Times.Never);
        }
    }

    [Fact]
    public async Task Handle_WhenNonExistingQuiz_ShouldThrowNotFoundException()
    {
        // Arrange

        var query = new GetQuizByIdQuery(Guid.NewGuid());

        _quizzesRepositoryMock.Setup(r => r.GetAsync(query.QuizId, true, false, false))
            .ReturnsAsync((Quiz)null!);

        // Act

        Func<Task> action = async () => await _handler.Handle(query, CancellationToken.None);

        // Assert

        var exception = await Assert.ThrowsAsync<NotFoundException>(action);
        Assert.Equal($"Quiz with ID {query.QuizId} was not found", exception.Message);
    }
}

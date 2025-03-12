using Moq;
using QuizAI.Application.Interfaces;
using QuizAI.Domain.Entities;
using QuizAI.Domain.Exceptions;
using QuizAI.Domain.Repositories;
using Xunit;

namespace QuizAI.Application.Quizzes.Commands.ChangeQuizPrivacy.Tests;

public class ChangeQuizPrivacyCommandHandlerTests
{
    private readonly Mock<IRepository> _repositoryMock = new();
    private readonly Mock<IQuizService> _quizServiceMock = new();
    private readonly Mock<IImagesRepository> _imagesRepositoryMock = new();
    private readonly Mock<IImageService> _imageServiceMock = new();

    private readonly ChangeQuizPrivacyCommandHandler _handler;

    public ChangeQuizPrivacyCommandHandlerTests()
    {
        _handler = new ChangeQuizPrivacyCommandHandler(_repositoryMock.Object, _quizServiceMock.Object, _imagesRepositoryMock.Object, _imageServiceMock.Object);
    }

    [Theory]
    [InlineData(true, true)]
    [InlineData(false, false)]
    public async Task Handle_WhenValidCommand_ShouldChangeQuizPrivacy(bool isPrivate, bool createdNewQuiz)
    {
        // Arrange

        var command = new ChangeQuizPrivacyCommand() { IsPrivate = isPrivate };
        command.SetId(Guid.NewGuid());

        var quizToUpdate = new Quiz() { Id = createdNewQuiz ? Guid.NewGuid() : command.GetId(), IsPrivate = !isPrivate };

        _quizServiceMock
            .Setup(s => s.GetValidOrDeprecateAndCreateWithNewQuestionsAsync(command.GetId()))
            .ReturnsAsync((quizToUpdate, createdNewQuiz));

        // Act

        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert

        Assert.Equal(result.latestQuizId, quizToUpdate.Id);
        Assert.Equal(command.IsPrivate, quizToUpdate.IsPrivate);

        if (createdNewQuiz)
        {
            _repositoryMock.Verify(r => r.AddAsync(quizToUpdate), Times.Once);
        }
        else
        {
            _repositoryMock.Verify(r => r.AddAsync(quizToUpdate), Times.Never);
        }

        _repositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task Handle_WhenTheSamePrivacy_ShouldThrowConflictException(bool isPrivate)
    {
        // Arrange

        var command = new ChangeQuizPrivacyCommand() { IsPrivate = isPrivate };
        command.SetId(Guid.NewGuid());

        var quizToUpdate = new Quiz() { Id = command.GetId(), IsPrivate = isPrivate };

        _quizServiceMock
            .Setup(s => s.GetValidOrDeprecateAndCreateWithNewQuestionsAsync(command.GetId()))
            .ReturnsAsync((quizToUpdate, false));

        // Act

        Func<Task> action = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert

        var exception = await Assert.ThrowsAsync<ConflictException>(action);
        Assert.Equal($"The privacy setting of quiz with ID {command.GetId()} is already set to {command.IsPrivate}", exception.Message);
    }
}

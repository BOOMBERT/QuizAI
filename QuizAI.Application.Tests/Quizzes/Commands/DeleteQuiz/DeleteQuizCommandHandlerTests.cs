using Moq;
using QuizAI.Application.Interfaces;
using QuizAI.Domain.Constants;
using QuizAI.Domain.Entities;
using QuizAI.Domain.Exceptions;
using QuizAI.Domain.Repositories;
using Xunit;

namespace QuizAI.Application.Quizzes.Commands.DeleteQuiz.Tests;

public class DeleteQuizCommandHandlerTests
{
    private readonly Mock<IRepository> _repositoryMock = new();
    private readonly Mock<IQuizAuthorizationService> _quizAuthorizationServiceMock = new();
    private readonly Mock<IQuizzesRepository> _quizzesRepositoryMock = new();
    private readonly Mock<IQuizAttemptsRepository> _quizAttemptsRepositoryMock = new();
    private readonly Mock<ICategoryService> _categoryServiceMock = new();
    private readonly Mock<IImageService> _imageServiceMock = new();
    private readonly Mock<IQuizPermissionsRepository> _quizPermissionsRepositoryMock = new();

    private readonly DeleteQuizCommandHandler _handler;

    public DeleteQuizCommandHandlerTests()
    {
        _handler = new DeleteQuizCommandHandler(
            _repositoryMock.Object, _quizAuthorizationServiceMock.Object, _quizzesRepositoryMock.Object, _quizAttemptsRepositoryMock.Object, 
            _categoryServiceMock.Object, _imageServiceMock.Object, _quizPermissionsRepositoryMock.Object);
    }

    [Theory]
    [InlineData(true, true)]
    [InlineData(false, false)]
    public async Task Handle_WhenValidCommand_ShouldDeleteOrDeprecateQuizAndRemoveImages(bool withImage, bool hasQuizAttempts)
    {
        // Arrange

        var command = new DeleteQuizCommand();
        command.SetId(Guid.NewGuid());

        var questionImageId = Guid.NewGuid();
        var quiz = new Quiz() 
        { 
            Id = command.GetId(),
            ImageId = withImage ? Guid.NewGuid() : null,
            Questions = new List<Question>
            {
                new Question { ImageId = questionImageId }
            }
        };

        _quizzesRepositoryMock
            .Setup(r => r.GetWithQuestionsAndCategoriesAsync(command.GetId()))
            .ReturnsAsync(quiz);

        _quizAttemptsRepositoryMock
            .Setup(r => r.HasAnyAsync(quiz.Id, null, null))
            .ReturnsAsync(hasQuizAttempts);

        // Act

        await _handler.Handle(command, CancellationToken.None);

        // Assert

        Assert.Null(quiz.ImageId);

        if (hasQuizAttempts) 
        {
            Assert.True(quiz.IsDeprecated);
            _quizPermissionsRepositoryMock.Verify(r => r.DeletePermissionsAsync(quiz.Id), Times.Once);
            Assert.Contains(quiz.Questions, qn => qn.ImageId == questionImageId);
        }
        else
        {
            _repositoryMock.Verify(r => r.Remove(quiz), Times.Once);
            _quizzesRepositoryMock.Verify(r => r.UpdateLatestVersionIdAsync(quiz.Id, null), Times.Once);
            Assert.True(quiz.Questions.All(qn => qn.ImageId == null));
        }

        _quizAuthorizationServiceMock.Verify(s => s.AuthorizeAsync(quiz, null, ResourceOperation.Delete), Times.Once);
        _repositoryMock.Verify(r => r.SaveChangesAsync(), Times.Exactly(2));
    }

    [Fact]
    public async Task Handle_WhenNonExistingQuiz_ShouldThrowNotFoundException()
    {
        // Arrange

        var command = new DeleteQuizCommand();
        command.SetId(Guid.NewGuid());

        _quizzesRepositoryMock
            .Setup(r => r.GetWithQuestionsAndCategoriesAsync(command.GetId()))
            .ReturnsAsync((Quiz?)null);

        // Act

        Func<Task> action = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert

        var exception = await Assert.ThrowsAsync<NotFoundException>(action);
        Assert.Equal($"Quiz with ID {command.GetId()} was not found", exception.Message);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task Handle_WhenDeprecatedQuiz_ShouldThrowNotFoundQuizWithVersioningException(bool withLatestVersionId)
    {
        // Arrange

        var command = new DeleteQuizCommand();
        command.SetId(Guid.NewGuid());

        var quiz = new Quiz() { Id = command.GetId(), IsDeprecated = true, LatestVersionId = withLatestVersionId ? Guid.NewGuid() : null };

        _quizzesRepositoryMock
            .Setup(r => r.GetWithQuestionsAndCategoriesAsync(command.GetId()))
            .ReturnsAsync(quiz);

        // Act

        Func<Task> action = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert

        var exception = await Assert.ThrowsAsync<NotFoundQuizWithVersioningException>(action);
        Assert.Equal($"The quiz with ID {quiz.Id} was not found - " + 
            (withLatestVersionId 
            ? $"The latest version ID is {quiz.LatestVersionId}" 
            : "No latest version is available"), 
            exception.Message);
    }
}

using AutoMapper;
using Moq;
using QuizAI.Application.Interfaces;
using QuizAI.Domain.Entities;
using QuizAI.Domain.Repositories;
using Xunit;

namespace QuizAI.Application.Quizzes.Commands.UpdateQuiz.Tests;

public class UpdateQuizCommandHandlerTests
{
    private readonly Mock<IQuizService> _quizServiceMock = new();
    private readonly Mock<ICategoryService> _categoryServiceMock = new();
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly Mock<IRepository> _repositoryMock = new();

    private readonly UpdateQuizCommandHandler _handler;

    public UpdateQuizCommandHandlerTests()
    {
        _handler = new UpdateQuizCommandHandler(_mapperMock.Object, _repositoryMock.Object, _quizServiceMock.Object, _categoryServiceMock.Object);
    }

    [Theory]
    [InlineData(false, new string[] { "test category 1", "test category 2" })]
    [InlineData(true, new string[] { "Test category" })]
    public async Task Handle_WhenValidCommand_ShouldUpdateQuizAndReturnItsId(bool createdNewQuiz, ICollection<string> quizCategories)
    {
        // Arrange

        var command = new UpdateQuizCommand() { Categories = quizCategories };
        command.SetId(Guid.NewGuid());
        
        var quizToUpdate = new Quiz() { Id = createdNewQuiz ? Guid.NewGuid() : command.GetId() };

        _quizServiceMock
            .Setup(s => s.GetValidOrDeprecateAndCreateWithNewQuestionsAsync(command.GetId()))
            .ReturnsAsync((quizToUpdate, createdNewQuiz));

        _categoryServiceMock
            .Setup(s => s.GetOrCreateEntitiesAsync(It.IsAny<IEnumerable<string>>()))
            .ReturnsAsync(command.Categories.Select(c => new Category { Name = c }).ToList());

        // Act

        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert

        Assert.Equal(quizToUpdate.Id, result.latestQuizId);
        Assert.Equal(command.Categories, quizToUpdate.Categories.Select(c => c.Name).ToList());

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
}

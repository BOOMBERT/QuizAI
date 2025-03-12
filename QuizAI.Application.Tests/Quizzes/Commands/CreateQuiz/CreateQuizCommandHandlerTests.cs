using AutoMapper;
using Microsoft.AspNetCore.Http;
using Moq;
using QuizAI.Application.Interfaces;
using QuizAI.Application.Users;
using QuizAI.Domain.Entities;
using QuizAI.Domain.Repositories;
using Xunit;

namespace QuizAI.Application.Quizzes.Commands.CreateQuiz.Tests;

public class CreateQuizCommandHandlerTests
{
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly Mock<IRepository> _repositoryMock = new();
    private readonly Mock<IImageService> _imageServiceMock = new();
    private readonly Mock<ICategoryService> _categoryServiceMock = new();
    private readonly Mock<IUserContext> _userContextMock = new();

    private readonly CreateQuizCommandHandler _handler;

    public CreateQuizCommandHandlerTests()
    {
        _handler = new CreateQuizCommandHandler(
            _mapperMock.Object, _repositoryMock.Object, _imageServiceMock.Object, _categoryServiceMock.Object, _userContextMock.Object);
    }

    [Theory]
    [InlineData(true, new string[] { "test category 1", "test category 2" })]
    [InlineData(false, new string[] { "Test category" })]
    public async Task Handle_WhenValidCommand_ShouldAssignPropertiesAndReturnCreatedQuizId(bool withImage, ICollection<string> quizCategories) 
    {
        // Arrange

        var command = new CreateQuizCommand
        {
            Image = withImage ? new Mock<IFormFile>().Object : null,
            Categories = quizCategories,
        };

        var quiz = new Quiz();

        var currentUser = new CurrentUser("creator-id", "test@test.com");
        _userContextMock
            .Setup(u => u.GetCurrentUser())
            .Returns(currentUser);

        _mapperMock
            .Setup(m => m.Map<Quiz>(command))
            .Returns(quiz);

        _categoryServiceMock
            .Setup(s => s.GetOrCreateEntitiesAsync(command.Categories))
            .ReturnsAsync(command.Categories.Select(c => new Category { Name = c }).ToList());

        _imageServiceMock
            .Setup(s => s.UploadAsync(command.Image!, command.IsPrivate))
            .ReturnsAsync(
                new Image
                {
                    Id = Guid.NewGuid(),
                    FileExtension = ".png",
                    Hash = new byte[] { 1, 2, 3 }
                }
            );

        _repositoryMock
            .Setup(r => r.AddAsync(quiz))
            .Callback<Quiz>(qz =>
            {
                qz.Id = Guid.NewGuid();

                if (qz.Image != null) qz.ImageId = qz.Image.Id;
            });

        // Act

        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert

        Assert.Equal(result, quiz.Id);
        Assert.NotEqual(Guid.Empty, quiz.Id);
        Assert.False(quiz.IsPrivate);
        Assert.False(quiz.IsDeprecated);
        Assert.Null(quiz.LatestVersionId);
        Assert.Equal(0, quiz.QuestionCount);
        Assert.Equal(currentUser.Id, quiz.CreatorId);
        Assert.Equal(command.Categories, quiz.Categories.Select(c => c.Name).ToList());

        if (withImage)
        {
            Assert.NotNull(quiz.Image);
            Assert.NotEqual(Guid.Empty, quiz.ImageId);
            Assert.Equal(quiz.Image.Id, quiz.ImageId);
            Assert.Equal(".png", quiz.Image.FileExtension);
            Assert.Equal(new byte[] { 1, 2, 3 }, quiz.Image.Hash);
            _imageServiceMock.Verify(s => s.UploadAsync(command.Image!, command.IsPrivate), Times.Once);
        }
        else
        {
            Assert.Null(quiz.Image);
            Assert.Null(quiz.ImageId);
            _imageServiceMock.Verify(s => s.UploadAsync(command.Image!, command.IsPrivate), Times.Never);
        }

        _repositoryMock.Verify(r => r.AddAsync(quiz), Times.Once);
        _repositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }
}

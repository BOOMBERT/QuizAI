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

        var mapperMock = new Mock<IMapper>();
        mapperMock
            .Setup(m => m.Map<Quiz>(command))
            .Returns(quiz);

        var repositoryMock = new Mock<IRepository>();
        repositoryMock
            .Setup(r => r.AddAsync(It.IsAny<Quiz>()))
            .Callback<Quiz>(qz =>
            {
                qz.Id = Guid.NewGuid();

                if (qz.Image != null) qz.ImageId = qz.Image.Id;
            });

        repositoryMock
            .Setup(r => r.SaveChangesAsync())
            .ReturnsAsync(true);

        var imageServiceMock = new Mock<IImageService>();
        Guid imageId = default;

        if (withImage)
        {
            imageId = Guid.NewGuid();

            imageServiceMock
                .Setup(s => s.UploadAsync(It.IsAny<IFormFile>(), It.IsAny<bool>()))
                .ReturnsAsync(
                    new Image
                    {
                        Id = imageId,
                        FileExtension = ".png",
                        Hash = new byte[] { 1, 2, 3 }
                    }
                );
        }

        var categoryServiceMock = new Mock<ICategoryService>();
        categoryServiceMock
            .Setup(s => s.GetOrCreateEntitiesAsync(It.IsAny<IEnumerable<string>>()))
            .ReturnsAsync(command.Categories.Select(c => new Category { Name = c }).ToList());

        var userContextMock = new Mock<IUserContext>();
        var currentUser = new CurrentUser("creator-id", "test@test.com");
        userContextMock
            .Setup(u => u.GetCurrentUser())
            .Returns(currentUser);

        var commandHandler = new CreateQuizCommandHandler(
            mapperMock.Object, repositoryMock.Object, imageServiceMock.Object, categoryServiceMock.Object, userContextMock.Object);

        // Act

        var result = await commandHandler.Handle(command, CancellationToken.None);

        // Assert

        Assert.Equal(result, quiz.Id);
        Assert.NotEqual(Guid.Empty, quiz.Id);
        Assert.False(quiz.IsPrivate);
        Assert.False(quiz.IsDeprecated);
        Assert.Null(quiz.LatestVersionId);
        Assert.Equal(0, quiz.QuestionCount);
        Assert.Equal(currentUser.Id, quiz.CreatorId);
        Assert.Equal(quizCategories, quiz.Categories.Select(c => c.Name).ToList());

        if (withImage)
        {
            Assert.NotNull(quiz.Image);
            Assert.NotEqual(Guid.Empty, quiz.ImageId);
            Assert.Equal(imageId, quiz.ImageId);
            Assert.Equal(imageId, quiz.Image!.Id);
            Assert.Equal(".png", quiz.Image.FileExtension);
            Assert.Equal(new byte[] { 1, 2, 3 }, quiz.Image.Hash);
            imageServiceMock.Verify(s => s.UploadAsync(It.IsAny<IFormFile>(), It.IsAny<bool>()), Times.Once);
        }
        else
        {
            Assert.Null(quiz.Image);
            Assert.Null(quiz.ImageId);
            imageServiceMock.Verify(s => s.UploadAsync(It.IsAny<IFormFile>(), It.IsAny<bool>()), Times.Never);
        }

        repositoryMock.Verify(r => r.AddAsync(quiz), Times.Once);
        repositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }
}

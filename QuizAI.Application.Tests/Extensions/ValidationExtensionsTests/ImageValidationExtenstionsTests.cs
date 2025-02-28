using FluentValidation;
using FluentValidation.TestHelper;
using Microsoft.AspNetCore.Http;
using Moq;
using QuizAI.Domain.Exceptions;
using Xunit;

namespace QuizAI.Application.Extensions.Tests;

public class ImageValidationExtensionsTests
{
    private InlineValidator<IFormFile?> GetIsValidImageValidator()
    {
        var validator = new InlineValidator<IFormFile?>();
        validator.RuleFor(x => x).IsValidImage();

        return validator;
    }

    [Theory]
    [InlineData("test.png", 1024 * 1024, "image/png", new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A })]
    [InlineData("test.jpg", 2 * 1024 * 1024, "image/jpeg", new byte[] { 0xFF, 0xD8, 0xFF })]
    [InlineData("test.jpeg", 3 * 1024 * 1024, "image/jpeg", new byte[] { 0xFF, 0xD8, 0xFF })]
    public void IsValidImage_WhenValidImage_ShouldNotHaveValidationErrors(string filename, int fileSize, string contentType, byte[] magicNumber)
    {
        // Arrange

        var imageMock = new Mock<IFormFile>();

        imageMock.Setup(f => f.FileName).Returns(filename);
        imageMock.Setup(f => f.Length).Returns(fileSize);
        imageMock.Setup(f => f.ContentType).Returns(contentType);
        imageMock.Setup(f => f.OpenReadStream()).Returns(new MemoryStream(magicNumber));

        // Act

        var exception = Record.Exception(() => GetIsValidImageValidator().TestValidate(imageMock.Object));

        // Assert
        
        Assert.Null(exception);
    }

    [Fact]
    public void IsValidImage_WhenEmptyImageFile_ShouldHaveValidationError()
    {
        // Arrange

        var imageMock = new Mock<IFormFile>();

        imageMock.Setup(f => f.Length).Returns(0);

        // Act

        Action action = () => GetIsValidImageValidator().TestValidate(imageMock.Object);

        // Assert

        var exception = Assert.Throws<UnprocessableEntityException>(action);
        Assert.Equal("The file is empty or missing", exception.Message);
    }

    [Theory]
    [InlineData(".gif")]
    [InlineData(".pdf")]
    public void IsValidImage_WhenUnsupportedExtensionOfImageFile_ShouldHaveValidationError(string fileExtension)
    {
        // Arrange

        var imageMock = new Mock<IFormFile>();

        imageMock.Setup(f => f.FileName).Returns($"test{fileExtension}");
        imageMock.Setup(f => f.Length).Returns(1);

        // Act

        Action action = () => GetIsValidImageValidator().TestValidate(imageMock.Object);

        // Assert

        var exception = Assert.Throws<UnsupportedMediaTypeException>(action);
        Assert.Equal($"The file extension {fileExtension} is not supported", exception.Message);
    }

    [Fact]
    public void IsValidImage_WhenImageFileExceedsMaxSize_ShouldHaveValidationError()
    {
        // Arrange

        var imageMock = new Mock<IFormFile>();

        imageMock.Setup(f => f.Length).Returns(6 * 1024 * 1024);
        imageMock.Setup(f => f.FileName).Returns("test.png");

        // Act

        Action action = () => GetIsValidImageValidator().TestValidate(imageMock.Object);

        // Assert

        var exception = Assert.Throws<RequestEntityTooLargeException>(action);
        Assert.Equal("The image file exceeds the maximum allowed size of 5 MB", exception.Message);
    }

    [Theory]
    [InlineData("application/pdf", "test.png")]
    [InlineData("image/jpeg", "test.png")]
    [InlineData("image/png", "test.jpeg")]
    public void IsValidImage_WhenInvalidImageContentType_ShouldHaveValidationError(string contentType, string filename)
    {
        // Arrange

        var imageMock = new Mock<IFormFile>();

        imageMock.Setup(f => f.ContentType).Returns(contentType);
        imageMock.Setup(f => f.FileName).Returns(filename);
        imageMock.Setup(f => f.Length).Returns(1);

        // Act

        Action action = () => GetIsValidImageValidator().TestValidate(imageMock.Object);

        // Assert

        var exception = Assert.Throws<UnprocessableEntityException>(action);
        Assert.Equal("Invalid image file", exception.Message);
    }

       
    [Theory]
    [InlineData("test.png", "image/png", new byte[] { 0xFF, 0xD8, 0xFF })]
    [InlineData("test.jpg", "image/jpeg", new byte[] { 0xAA, 0xAA, 0xAA })]
    public void IsValidImage_WhenInvalidImageFileMagicNumber_ShouldHaveValidationError(string filename, string contentType, byte[] magicNumber)
    {
        // Arrange

        var imageMock = new Mock<IFormFile>();

        imageMock.Setup(f => f.ContentType).Returns(contentType);
        imageMock.Setup(f => f.FileName).Returns(filename);
        imageMock.Setup(f => f.OpenReadStream()).Returns(new MemoryStream(magicNumber));
        imageMock.Setup(f => f.Length).Returns(1);

        // Act

        Action action = () => GetIsValidImageValidator().TestValidate(imageMock.Object);

        // Assert

        var exception = Assert.Throws<UnprocessableEntityException>(action);
        Assert.Equal("Invalid image file", exception.Message);
    }   
}

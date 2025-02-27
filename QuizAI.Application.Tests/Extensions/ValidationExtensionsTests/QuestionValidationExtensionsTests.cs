using FluentValidation;
using FluentValidation.TestHelper;
using Xunit;

namespace QuizAI.Application.Extensions.Tests;

public class QuestionValidationExtensionsTests
{
    private readonly InlineValidator<string> _IsValidQuestionContentValidator;

    public QuestionValidationExtensionsTests()
    {
        _IsValidQuestionContentValidator = new InlineValidator<string>();
        _IsValidQuestionContentValidator.RuleFor(x => x).IsValidQuestionContent();
    }

    [Fact]
    public void IsValidQuestionContent_WhenValidQuestionContent_ShouldNotHaveValidationErrors()
    {
        // Arrange

        var validQuestionContent = "Valid Question Content";

        // Act

        var result = _IsValidQuestionContentValidator.TestValidate(validQuestionContent);

        // Assert

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void IsValidQuestionContent_WhenEmptyOrWhitespaceQuestionContent_ShouldHaveValidationError(string questionContent)
    {
        // Act

        var result = _IsValidQuestionContentValidator.TestValidate(questionContent);

        // Assert

        result.ShouldHaveValidationErrorFor(x => x)
            .WithErrorMessage("Question content cannot be empty or whitespace");
    }

    [Fact]
    public void IsValidQuestionContent_WhenTooLongQuestionContent_ShouldHaveValidationError()
    {
        // Arrange

        var tooLongQuestionContent = new string('A', 513);

        // Act

        var result = _IsValidQuestionContentValidator.TestValidate(tooLongQuestionContent);

        // Assert

        result.ShouldHaveValidationErrorFor(x => x)
            .WithErrorMessage("Question content must be at most 512 characters long");
    }
}

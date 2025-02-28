using FluentValidation;
using FluentValidation.TestHelper;
using Xunit;

namespace QuizAI.Application.Extensions.Tests;

public class QuestionValidationExtensionsTests
{
    private InlineValidator<string> GetIsValidQuestionContentValidator()
    {
        var validator = new InlineValidator<string>();
        validator.RuleFor(x => x).IsValidQuestionContent();

        return validator;
    }

    [Fact]
    public void IsValidQuestionContent_WhenValidQuestionContent_ShouldNotHaveValidationErrors()
    {
        // Arrange

        var validQuestionContent = "Valid Question Content";

        // Act

        var result = GetIsValidQuestionContentValidator().TestValidate(validQuestionContent);

        // Assert

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void IsValidQuestionContent_WhenEmptyOrWhitespaceQuestionContent_ShouldHaveValidationError(string questionContent)
    {
        // Act

        var result = GetIsValidQuestionContentValidator().TestValidate(questionContent);

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

        var result = GetIsValidQuestionContentValidator().TestValidate(tooLongQuestionContent);

        // Assert

        result.ShouldHaveValidationErrorFor(x => x)
            .WithErrorMessage("Question content must be at most 512 characters long");
    }
}

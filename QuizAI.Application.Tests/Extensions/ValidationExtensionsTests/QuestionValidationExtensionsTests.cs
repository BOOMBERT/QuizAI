using FluentValidation;
using FluentValidation.TestHelper;
using Xunit;

namespace QuizAI.Application.Extensions.Tests;

public class QuestionValidationExtensionsTests
{
    [Fact()]
    public void IsValidQuestionContent_WhenValidQuestionContent_ShouldNotHaveValidationErrors()
    {
        // arrange

        var validator = new InlineValidator<string>();
        validator.RuleFor(x => x).IsValidQuestionContent();

        // act

        var result = validator.TestValidate("Valid Question Content");

        // assert

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory()]
    [InlineData("")]
    [InlineData(" ")]
    public void IsValidQuestionContent_WhenEmptyOrWhitespaceQuestionContent_ShouldHaveValidationError(string questionContent)
    {
        // arrange

        var validator = new InlineValidator<string>();
        validator.RuleFor(x => x).IsValidQuestionContent();

        // act

        var result = validator.TestValidate(questionContent);

        // assert

        result.ShouldHaveValidationErrorFor(x => x)
            .WithErrorMessage("Question content cannot be empty or whitespace");
    }

    [Fact()]
    public void IsValidQuestionContent_WhenTooLongQuestionContent_ShouldHaveValidationError()
    {
        // arrange

        var validator = new InlineValidator<string>();
        validator.RuleFor(x => x).IsValidQuestionContent();

        // act

        var result = validator.TestValidate(new string('A', 513));

        // assert

        result.ShouldHaveValidationErrorFor(x => x)
            .WithErrorMessage("Question content must be at most 512 characters long");
    }
}

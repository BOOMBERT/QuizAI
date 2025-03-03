using FluentValidation.TestHelper;
using QuizAI.Application.Tests.TestHelpers;
using Xunit;

namespace QuizAI.Application.Questions.Commands.AnswerCurrentQuestion.Tests;

public class AnswerCurrentQuestionCommandValidatorTests
{
    private (AnswerCurrentQuestionCommand, AnswerCurrentQuestionCommandValidator) GetAnswerCurrentQuestionCommandAndValidator(IList<string> userAnswer) 
        => (new AnswerCurrentQuestionCommand() { UserAnswer =  userAnswer }, new AnswerCurrentQuestionCommandValidator());

    [Theory]
    [InlineData((object)new string[] { "test" })]
    [InlineData((object)new string[] { "test 1", "test 2" })]
    [InlineData((object)new string[] { "test", "Test" })]
    [InlineData((object)new string[] { "true" })]
    [InlineData((object)new string[] { "1", "2", "3", "4", "5", "6", "7", "8" })]
    public void Validator_WhenValidCommand_ShouldNotHaveValidationErrors(IList<string> userValidAnswer)
    {
        // Arrange

        var (command, validator) = GetAnswerCurrentQuestionCommandAndValidator(userValidAnswer);

        // Act

        var result = validator.TestValidate(command);

        // Assert

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData((object)new string[] { "" } )]
    [InlineData((object)new string[] { " " })]
    [InlineData((object)new string[] { "test", "" })]
    [InlineData((object)new string[] { "test", " " })]
    public void Validator_WhenEmptyOrWhitespaceUserAnswer_ShouldHaveValidationError(IList<string> userAnswer)
    {
        // Arrange

        var (command, validator) = GetAnswerCurrentQuestionCommandAndValidator(userAnswer);

        // Act

        var result = validator.TestValidate(command);


        // Assert

        result.ShouldHaveValidationErrorFor(x => x.UserAnswer)
            .WithErrorMessage("Answer cannot be empty or whitespace");

    }

    [Theory]
    [InlineData((object)new string[] { "test", "test" })]
    [InlineData((object)new string[] { "test", "test 2", "test" })]
    public void Validator_WhenNotUniqueUserAnswer_ShoudHaveValidationError(IList<string> userAnswer)
    {
        // Arrange

        var (command, validator) = GetAnswerCurrentQuestionCommandAndValidator(userAnswer);

        // Act

        var result = validator.TestValidate(command);


        // Assert

        result.ShouldHaveValidationErrorFor(x => x.UserAnswer)
            .WithErrorMessage("Answers must be unique");
    }

    [Fact]
    public void Validator_WhenTooManyUserAnswer_ShouldHaveValidationError()
    {
        // Arrange

        var (command, validator) = GetAnswerCurrentQuestionCommandAndValidator(new string[] { "1", "2", "3", "4", "5", "6", "7", "8", "9" });

        // Act

        var result = validator.TestValidate(command);


        // Assert

        result.ShouldHaveValidationErrorFor(x => x.UserAnswer)
            .WithErrorMessage("You can specify up to 8 answers");
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(8)]
    public void Validator_WhenTooLongTotalLengthOfUserAnswer_ShouldHaveValidationError(int numberOfAnswers)
    {
        // Arrange

        var userAnswerWithTooLongTotalLength = AnswerTestHelper.GenerateAnswersExceedingMaxTotalLength(numberOfAnswers, 2040);

        var (command, validator) = GetAnswerCurrentQuestionCommandAndValidator(userAnswerWithTooLongTotalLength);

        // Act

        var result = validator.TestValidate(command);

        // Assert

        result.ShouldHaveValidationErrorFor(x => x.UserAnswer)
            .WithErrorMessage("The total length of all answers must not exceed 2040 characters");
    }
}
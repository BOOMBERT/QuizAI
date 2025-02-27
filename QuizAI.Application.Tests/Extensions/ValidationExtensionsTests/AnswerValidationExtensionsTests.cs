using FluentValidation;
using FluentValidation.TestHelper;
using QuizAI.Application.MultipleChoiceQuestions.Dtos;
using Xunit;

namespace QuizAI.Application.Extensions.Tests;

public class AnswerValidationExtensionsTests
{
    private readonly InlineValidator<ICollection<string>> _IsValidOpenEndedAnswersValidator;

    public AnswerValidationExtensionsTests()
    {
        _IsValidOpenEndedAnswersValidator = new InlineValidator<ICollection<string>>();
        _IsValidOpenEndedAnswersValidator.RuleFor(x => x).IsValidOpenEndedAnswers();
    }

    #region Test Multiple Choice Answers

    private (InlineValidator<ICollection<MultipleChoiceAnswerDto>>, ICollection<MultipleChoiceAnswerDto>) GetValidatorAndMultipleChoiceAnswerDtos(string[] answersContent)
    {
        var validator = new InlineValidator<ICollection<MultipleChoiceAnswerDto>>();
        validator.RuleFor(x => x).IsValidMultipleChoiceAnswers();

        var multipleChoiceAnswerDtos = answersContent.Select(ac => new MultipleChoiceAnswerDto(ac, true)).ToList();

        return (validator, multipleChoiceAnswerDtos);
    }

    [Theory]
    [InlineData((object)new string[] { "test 1", "test 2" })]
    [InlineData((object)new string[] { "test", "Test" })]
    [InlineData((object)new string[] { "1", "2", "3", "4", "5", "6", "7", "8" })]
    public void IsValidMultipleChoiceAnswers_WhenValidMultipleChoiceAnswers_ShouldNotHaveValidationErrors(string[] answersContent)
    {
        // Arrange

        var (validator, validMultipleChoiceAnswerDtos) = GetValidatorAndMultipleChoiceAnswerDtos(answersContent);

        // Act

        var result = validator.TestValidate(validMultipleChoiceAnswerDtos);

        // Assert

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData((object)new string[] { })]
    [InlineData((object)new string[] { "test" })]
    public void IsValidMultipleChoiceAnswers_WhenNotEnoughMultipleChoiceAnswers_ShouldHaveValidationError(string[] answersContent)
    {
        // Arrange

        var (validator, multipleChoiceAnswerDtos) = GetValidatorAndMultipleChoiceAnswerDtos(answersContent);

        // Act

        var result = validator.TestValidate(multipleChoiceAnswerDtos);

        // Assert

        result.ShouldHaveValidationErrorFor(x => x)
            .WithErrorMessage("At least two answers are required");
    }

    [Fact]
    public void IsValidMultipleChoiceAnswers_WhenTooManyMultipleChoiceAnswers_ShouldHaveValidationError()
    {
        // Arrange

        var (validator, multipleChoiceAnswerDtos) = GetValidatorAndMultipleChoiceAnswerDtos(new string[] { "1", "2", "3", "4", "5", "6", "7", "8", "9" });

        // Act

        var result = validator.TestValidate(multipleChoiceAnswerDtos);

        // Assert

        result.ShouldHaveValidationErrorFor(x => x)
            .WithErrorMessage("You can specify up to 8 answers");
    }

    [Theory]
    [InlineData((object)new string[] { "test", "test" })]
    [InlineData((object)new string[] { "test", "test 2", "test" })]
    public void IsValidMultipleChoiceAnswers_WhenNotUniqueMultipleChoiceAnswers_ShouldHaveValidationError(string[] answersContent)
    {
        // Arrange

        var (validator, multipleChoiceAnswerDtos) = GetValidatorAndMultipleChoiceAnswerDtos(answersContent);

        // Act

        var result = validator.TestValidate(multipleChoiceAnswerDtos);

        // Assert

        result.ShouldHaveValidationErrorFor(x => x)
            .WithErrorMessage("Answers must be unique");
    }

    [Theory]
    [InlineData((object)new string[] { "test", "" })]
    [InlineData((object)new string[] { "test", " "})]
    public void IsValidMultipleChoiceAnswers_WhenEmptyOrWhitespaceAnyMultipleChoiceAnswer_ShouldHaveValidationError(string[] answersContent)
    {
        // Arrange

        var (validator, multipleChoiceAnswerDtos) = GetValidatorAndMultipleChoiceAnswerDtos(answersContent);

        // Act

        var result = validator.TestValidate(multipleChoiceAnswerDtos);

        // Assert

        result.ShouldHaveValidationErrorFor(x => x)
            .WithErrorMessage("Answer cannot be empty or whitespace");
    }

    [Fact]
    public void IsValidMultipleChoiceAnswers_WhenTooLongAnyMultipleChoiceAnswer_ShouldHaveValidationError()
    {
        // Arrange

        var (validator, multipleChoiceAnswerDtos) = GetValidatorAndMultipleChoiceAnswerDtos(new string[] { new string('A', 256), "test" });

        // Act

        var result = validator.TestValidate(multipleChoiceAnswerDtos);

        // Assert

        result.ShouldHaveValidationErrorFor(x => x)
            .WithErrorMessage("Each answer content must not exceed 255 characters");
    }

    #endregion

    #region Test Open Ended Answers
    
    [Theory]
    [InlineData((object)new string[] { "test" })]
    [InlineData((object)new string[] { "test", "Test" })]
    [InlineData((object)new string[] { "test", "test " })]
    [InlineData((object)new string[] { "test 1", "test 2" })]
    public void IsValidOpenEndedAnswers_WhenValidOpenEndedAnswers_ShouldNotHaveValidationErrors(string[] openEndedAnswers)
    {        
        // Act
        
        var result = _IsValidOpenEndedAnswersValidator.TestValidate(openEndedAnswers);

        // Assert

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void IsValidOpenEndedAnswers_WhenEmptyOpenEndedAnswers_ShouldHaveValidationError()
    {
        // Arrange

        var emptyOpenEndedAnswers = Array.Empty<string>();

        // Act

        var result = _IsValidOpenEndedAnswersValidator.TestValidate(emptyOpenEndedAnswers);

        // Assert

        result.ShouldHaveValidationErrorFor(x => x)
            .WithErrorMessage("At least one answer is required");
    }

    [Fact]
    public void IsValidOpenEndedAnswers_WhenTooManyOpenEndedAnswers_ShouldHaveValidationError()
    {
        // Arrange

        var tooManyOpenEndedAnswers = Enumerable.Range(1, 21).Select(x => x.ToString()).ToList();

        // Act

        var result = _IsValidOpenEndedAnswersValidator.TestValidate(tooManyOpenEndedAnswers);

        // Assert

        result.ShouldHaveValidationErrorFor(x => x)
            .WithErrorMessage("You can specify up to 20 answers");
    }

    [Theory]
    [InlineData((object)new string[] { "test", "test" })]
    [InlineData((object)new string[] { "test", "test 2", "test" })]
    public void IsValidOpenEndedAnswers_WhenNotUniqueOpenEndedAnswers_ShouldHaveValidationError(string[] openEndedAnswers)
    {
        // Act

        var result = _IsValidOpenEndedAnswersValidator.TestValidate(openEndedAnswers);

        // Assert

        result.ShouldHaveValidationErrorFor(x => x)
            .WithErrorMessage("Answers must be unique");
    }

    [Theory]
    [InlineData((object)new string[] { "" })]
    [InlineData((object)new string[] { " " })]
    [InlineData((object)new string[] { "test", "" })]
    [InlineData((object)new string[] { "test", " " })]
    public void IsValidOpenEndedAnswers_WhenEmptyOrWhitespaceAnyOpenEndedAnswer_ShouldHaveValidationError(string[] openEndedAnswers)
    {
        // Act

        var result = _IsValidOpenEndedAnswersValidator.TestValidate(openEndedAnswers);

        // Assert

        result.ShouldHaveValidationErrorFor(x => x)
            .WithErrorMessage("Answer cannot be empty or whitespace");
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(20)]
    public void IsValidOpenEndedAnswers_WhenTooLongTotalLengthOfOpenEndedAnswers_ShouldHaveValidationError(int numberOfAnswers)
    {
        // Arrange

        int lengthPerAnswer = (int)Math.Ceiling(1276.0 / numberOfAnswers);
        var openEndedAnswersWithTooLongTotalLength = Enumerable.Repeat(new string('A', lengthPerAnswer), numberOfAnswers).ToList();

        // Act

        var result = _IsValidOpenEndedAnswersValidator.TestValidate(openEndedAnswersWithTooLongTotalLength);

        // Assert

        result.ShouldHaveValidationErrorFor(x => x)
            .WithErrorMessage("The total length of all answers must not exceed 1275 characters");
    }

    #endregion
}

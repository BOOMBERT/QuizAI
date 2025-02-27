using FluentValidation;
using FluentValidation.TestHelper;
using QuizAI.Application.MultipleChoiceQuestions.Dtos;
using Xunit;

namespace QuizAI.Application.Extensions.Tests;

public class AnswerValidationExtensionsTests
{
    #region Test Multiple Choice Answers

    private (InlineValidator<ICollection<MultipleChoiceAnswerDto>>, ICollection<MultipleChoiceAnswerDto>) GetValidatorAndMultipleChoiceAnswerDtos(string[] answersContent)
    {
        var validator = new InlineValidator<ICollection<MultipleChoiceAnswerDto>>();
        validator.RuleFor(x => x).IsValidMultipleChoiceAnswers();

        var multipleChoiceAnswerDtos = answersContent.Select(ac => new MultipleChoiceAnswerDto(ac, true)).ToList();

        return (validator, multipleChoiceAnswerDtos);
    }

    [Theory()]
    [InlineData((object)new string[] { "test 1", "test 2" })]
    [InlineData((object)new string[] { "test", "Test" })]
    [InlineData((object)new string[] { "1", "2", "3", "4", "5", "6", "7", "8" })]
    public void IsValidMultipleChoiceAnswers_WhenValidMultipleChoiceAnswers_ShouldNotHaveValidationErrors(string[] answersContent)
    {
        // arrange

        var (validator, validMultipleChoiceAnswerDtos) = GetValidatorAndMultipleChoiceAnswerDtos(answersContent);

        // act

        var result = validator.TestValidate(validMultipleChoiceAnswerDtos);

        // assert

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory()]
    [InlineData((object)new string[] { })]
    [InlineData((object)new string[] { "test" })]
    public void IsValidMultipleChoiceAnswers_WhenNotEnoughMultipleChoiceAnswers_ShouldHaveValidationError(string[] answersContent)
    {
        // arrange

        var (validator, multipleChoiceAnswerDtos) = GetValidatorAndMultipleChoiceAnswerDtos(answersContent);

        // act

        var result = validator.TestValidate(multipleChoiceAnswerDtos);

        // assert

        result.ShouldHaveValidationErrorFor(x => x)
            .WithErrorMessage("At least two answers are required");
    }

    [Fact()]
    public void IsValidMultipleChoiceAnswers_WhenTooManyMultipleChoiceAnswers_ShouldHaveValidationError()
    {
        // arrange

        var (validator, multipleChoiceAnswerDtos) = GetValidatorAndMultipleChoiceAnswerDtos(new string[] { "1", "2", "3", "4", "5", "6", "7", "8", "9" });

        // act

        var result = validator.TestValidate(multipleChoiceAnswerDtos);

        // assert

        result.ShouldHaveValidationErrorFor(x => x)
            .WithErrorMessage("You can specify up to 8 answers");
    }

    [Theory()]
    [InlineData((object)new string[] { "test", "test" })]
    [InlineData((object)new string[] { "test", "test 2", "test" })]
    public void IsValidMultipleChoiceAnswers_WhenNotUniqueMultipleChoiceAnswers_ShouldHaveValidationError(string[] answersContent)
    {
        // arrange

        var (validator, multipleChoiceAnswerDtos) = GetValidatorAndMultipleChoiceAnswerDtos(answersContent);

        // act

        var result = validator.TestValidate(multipleChoiceAnswerDtos);

        // assert

        result.ShouldHaveValidationErrorFor(x => x)
            .WithErrorMessage("Answers must be unique");
    }

    [Theory()]
    [InlineData((object)new string[] { "test", "" })]
    [InlineData((object)new string[] { "test", " "})]
    public void IsValidMultipleChoiceAnswers_WhenEmptyOrWhitespaceAnyMultipleChoiceAnswer_ShouldHaveValidationError(string[] answersContent)
    {
        // arrange

        var (validator, multipleChoiceAnswerDtos) = GetValidatorAndMultipleChoiceAnswerDtos(answersContent);

        // act

        var result = validator.TestValidate(multipleChoiceAnswerDtos);

        // assert

        result.ShouldHaveValidationErrorFor(x => x)
            .WithErrorMessage("Answer cannot be empty or whitespace");
    }

    [Fact()]
    public void IsValidMultipleChoiceAnswers_WhenTooLongAnyMultipleChoiceAnswer_ShouldHaveValidationError()
    {
        // arrange

        var (validator, multipleChoiceAnswerDtos) = GetValidatorAndMultipleChoiceAnswerDtos(new string[] { new string('A', 256), "test" });

        // act

        var result = validator.TestValidate(multipleChoiceAnswerDtos);

        // assert

        result.ShouldHaveValidationErrorFor(x => x)
            .WithErrorMessage("Each answer content must not exceed 255 characters");
    }

    #endregion

    #region Test Open Ended Answers
    
    [Theory()]
    [InlineData((object)new string[] { "test" })]
    [InlineData((object)new string[] { "test", "Test" })]
    [InlineData((object)new string[] { "test", "test " })]
    [InlineData((object)new string[] { "test 1", "test 2" })]
    public void IsValidOpenEndedAnswers_WhenValidOpenEndedAnswers_ShouldNotHaveValidationErrors(string[] openEndedAnswers)
    {
        // arrange

        var validator = new InlineValidator<ICollection<string>>();
        validator.RuleFor(x => x).IsValidOpenEndedAnswers();
        
        // act
        
        var result = validator.TestValidate(openEndedAnswers);

        // assert

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact()]
    public void IsValidOpenEndedAnswers_WhenEmptyOpenEndedAnswers_ShouldHaveValidationError()
    {
        // arrange

        var validator = new InlineValidator<ICollection<string>>();
        validator.RuleFor(x => x).IsValidOpenEndedAnswers();

        // act

        var result = validator.TestValidate(Array.Empty<string>());

        // assert

        result.ShouldHaveValidationErrorFor(x => x)
            .WithErrorMessage("At least one answer is required");
    }

    [Fact()]
    public void IsValidOpenEndedAnswers_WhenTooManyOpenEndedAnswers_ShouldHaveValidationError()
    {
        // arrange

        var validator = new InlineValidator<ICollection<string>>();
        validator.RuleFor(x => x).IsValidOpenEndedAnswers();

        var listWith21Items = Enumerable.Range(1, 21).Select(x => x.ToString()).ToList();

        // act

        var result = validator.TestValidate(listWith21Items);

        // assert

        result.ShouldHaveValidationErrorFor(x => x)
            .WithErrorMessage("You can specify up to 20 answers");
    }

    [Theory()]
    [InlineData((object)new string[] { "test", "test" })]
    [InlineData((object)new string[] { "test", "test 2", "test" })]
    public void IsValidOpenEndedAnswers_WhenNotUniqueOpenEndedAnswers_ShouldHaveValidationError(string[] openEndedAnswers)
    {
        // arrange

        var validator = new InlineValidator<ICollection<string>>();
        validator.RuleFor(x => x).IsValidOpenEndedAnswers();

        // act

        var result = validator.TestValidate(openEndedAnswers);

        // assert

        result.ShouldHaveValidationErrorFor(x => x)
            .WithErrorMessage("Answers must be unique");
    }

    [Theory()]
    [InlineData((object)new string[] { "" })]
    [InlineData((object)new string[] { " " })]
    [InlineData((object)new string[] { "test", "" })]
    [InlineData((object)new string[] { "test", " " })]
    public void IsValidOpenEndedAnswers_WhenEmptyOrWhitespaceAnyOpenEndedAnswer_ShouldHaveValidationError(string[] openEndedAnswers)
    {
        // arrange

        var validator = new InlineValidator<ICollection<string>>();
        validator.RuleFor(x => x).IsValidOpenEndedAnswers();

        // act

        var result = validator.TestValidate(openEndedAnswers);

        // assert

        result.ShouldHaveValidationErrorFor(x => x)
            .WithErrorMessage("Answer cannot be empty or whitespace");
    }

    [Theory()]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(20)]
    public void IsValidOpenEndedAnswers_WhenTooLongTotalLengthOfOpenEndedAnswers_ShouldHaveValidationError(int numberOfAnswers)
    {
        // arrange

        var validator = new InlineValidator<ICollection<string>>();
        validator.RuleFor(x => x).IsValidOpenEndedAnswers();

        int lengthPerAnswer = (int)Math.Ceiling(1276.0 / numberOfAnswers);
        var openEndedAnswers = Enumerable.Repeat(new string('A', lengthPerAnswer), numberOfAnswers).ToList();

        // act

        var result = validator.TestValidate(openEndedAnswers);

        // assert

        result.ShouldHaveValidationErrorFor(x => x)
            .WithErrorMessage("The total length of all answers must not exceed 1275 characters");
    }

    #endregion
}

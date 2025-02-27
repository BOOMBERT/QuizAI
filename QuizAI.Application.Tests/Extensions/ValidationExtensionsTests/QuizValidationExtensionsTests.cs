using FluentValidation;
using FluentValidation.TestHelper;
using Xunit;

namespace QuizAI.Application.Extensions.Tests;

public class QuizValidationExtensionsTests
{
    #region Test Quiz Name

    [Fact()]
    public void IsValidQuizName_WhenValidQuizName_ShouldNotHaveValidationErrors()
    {
        // arrange

        var validator = new InlineValidator<string>();
        validator.RuleFor(x => x).IsValidQuizName();

        // act

        var result = validator.TestValidate("Valid Quiz Name");

        // assert

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact()]
    public void IsValidQuizName_WhenTooLongQuizName_ShouldHaveValidationError()
    {
        // arrange

        var validator = new InlineValidator<string>();
        validator.RuleFor(x => x).IsValidQuizName();

        // act

        var result = validator.TestValidate(new string('A', 129));

        // assert

        result.ShouldHaveValidationErrorFor(x => x)
            .WithErrorMessage("Quiz name must be at most 128 characters long");
    }

    [Theory()]
    [InlineData("")]
    [InlineData(" ")]
    public void IsValidQuizName_WhenEmptyOrWhitespaceQuizName_ShouldHaveValidationError(string quizName)
    {
        // arrange

        var validator = new InlineValidator<string>();
        validator.RuleFor(x => x).IsValidQuizName();

        // act

        var result = validator.TestValidate(quizName);

        // assert

        result.ShouldHaveValidationErrorFor(x => x)
            .WithErrorMessage("Quiz name cannot be empty or whitespace");
    }

    #endregion

    #region Test Quiz Description

    [Fact()]
    public void IsValidQuizDescription_WhenValidQuizDescription_ShouldNotHaveValidationErrors()
    {
        // arrange

        var validator = new InlineValidator<string?>();
        validator.RuleFor(x => x).IsValidQuizDescription();

        // act

        var result = validator.TestValidate("Valid Quiz Description");

        // assert

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact()]
    public void IsValidQuizDescription_WhenTooLongQuizDescription_ShouldHaveValidationError()
    {
        // arrange
        
        var validator = new InlineValidator<string?>();
        validator.RuleFor(x => x).IsValidQuizDescription();

        // act

        var result = validator.TestValidate(new string('A', 513));

        // assert

        result.ShouldHaveValidationErrorFor(x => x)
            .WithErrorMessage("Quiz description must be at most 512 characters long");
    }

    [Theory()]
    [InlineData("")]
    [InlineData(" ")]
    public void IsValidQuizDescription_WhenEmptyOrWhitespaceQuizDescription_ShouldHaveValidationError(string quizDescription)
    {
        // arrange

        var validator = new InlineValidator<string?>();
        validator.RuleFor(x => x).IsValidQuizDescription();

        // act

        var result = validator.TestValidate(quizDescription);

        // assert

        result.ShouldHaveValidationErrorFor(x => x)
            .WithErrorMessage("Quiz description cannot be empty or whitespace");
    }

    #endregion

    #region Test Quiz Categories

    [Theory()]
    [InlineData((object)new string[] { "test" })]
    [InlineData((object)new string[] { "test 1", "test 2" })]
    [InlineData((object)new string[] { "1", "2", "3", "4", "5", "6", "7", "8", "9", "10" })]
    public void IsValidQuizCategories_WhenValidQuizCategories_ShouldNotHaveValidationErrors(ICollection<string> quizCategories)
    {
        // arrange

        var validator = new InlineValidator<ICollection<string>>();
        validator.RuleFor(x => x).IsValidQuizCategories();

        // act

        var result = validator.TestValidate(quizCategories);

        // assert

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact()]
    public void IsValidQuizCategories_WhenEmptyQuizCategories_ShouldHaveValidationError()
    {
        // arrange

        var validator = new InlineValidator<ICollection<string>>();
        validator.RuleFor(x => x).IsValidQuizCategories();

        // act

        var result = validator.TestValidate(Array.Empty<string>());

        // assert

        result.ShouldHaveValidationErrorFor(x => x)
            .WithErrorMessage("At least one category is required");
    }

    [Fact()]
    public void IsValidQuizCategories_WhenTooManyQuizCategories_ShouldHaveValidationError()
    {
        // arrange

        var validator = new InlineValidator<ICollection<string>>();
        validator.RuleFor(x => x).IsValidQuizCategories();

        // act

        var result = validator.TestValidate(new string[] { "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11" });

        // assert

        result.ShouldHaveValidationErrorFor(x => x)
            .WithErrorMessage("You can specify up to 10 categories");
    }

    [Theory()]
    [InlineData((object)new string[] { "" } )]
    [InlineData((object)new string[] { " " })]
    [InlineData((object)new string[] { "test", "" })]
    [InlineData((object)new string[] { "test", " " })]
    public void IsValidQuizCategories_WhenEmptyOrWhitespaceAnyQuizCategory_ShouldHaveValidationError(ICollection<string> quizCategories)
    {
        // arrange

        var validator = new InlineValidator<ICollection<string>>();
        validator.RuleFor(x => x).IsValidQuizCategories();

        // act

        var result = validator.TestValidate(quizCategories);

        // assert

        result.ShouldHaveValidationErrorFor(x => x)
            .WithErrorMessage("Category cannot be empty or whitespace");
    }

    [Theory()]
    [InlineData((object)new string[] { "test", "test" })]
    [InlineData((object)new string[] { "test", "TEST" })]
    [InlineData((object)new string[] { "test", "TeSt" })]
    [InlineData((object)new string[] { "test", " t e s t " })]
    [InlineData((object)new string[] { "test", "test 2", "test" })]
    public void IsValidQuizCategories_WhenNotUniqueQuizCategoriesRegardlessOfCaseAndSpaces_ShouldHaveValidationError(ICollection<string> quizCategories)
    {
        // arrange

        var validator = new InlineValidator<ICollection<string>>();
        validator.RuleFor(x => x).IsValidQuizCategories();

        // act

        var result = validator.TestValidate(quizCategories);

        // assert

        result.ShouldHaveValidationErrorFor(x => x)
            .WithErrorMessage("Categories must be unique");
    }

    [Theory()]
    [InlineData((object)new string[] { })]
    [InlineData((object)new string[] { "test" })]
    public void IsValidQuizCategories_WhenTooLongAnyQuizCategory_ShouldHaveValidationError(ICollection<string> quizCategories)
    {
        // arrange

        var validator = new InlineValidator<ICollection<string>>();
        validator.RuleFor(x => x).IsValidQuizCategories();

        var quizCategoriesList = quizCategories.ToList();
        quizCategoriesList.Add(new string('A', 65));

        // act

        var result = validator.TestValidate(quizCategoriesList);

        // assert

        result.ShouldHaveValidationErrorFor(x => x)
            .WithErrorMessage("Each category must not exceed 64 characters");
    }

    #endregion
}

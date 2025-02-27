using FluentValidation;
using FluentValidation.TestHelper;
using Xunit;

namespace QuizAI.Application.Extensions.Tests;

public class QuizValidationExtensionsTests
{
    private readonly InlineValidator<string> _isValidQuizNameValidator;
    private readonly InlineValidator<string?> _isValidQuizDescriptionValidator;
    private readonly InlineValidator<ICollection<string>> _isValidQuizCategoriesValidator;

    public QuizValidationExtensionsTests()
    {
        _isValidQuizNameValidator = new InlineValidator<string>();
        _isValidQuizNameValidator.RuleFor(x => x).IsValidQuizName();

        _isValidQuizDescriptionValidator = new InlineValidator<string?>();
        _isValidQuizDescriptionValidator.RuleFor(x => x).IsValidQuizDescription();

        _isValidQuizCategoriesValidator = new InlineValidator<ICollection<string>>();
        _isValidQuizCategoriesValidator.RuleFor(x => x).IsValidQuizCategories();
    }

    #region Test Quiz Name

    [Fact]
    public void IsValidQuizName_WhenValidQuizName_ShouldNotHaveValidationErrors()
    {
        // Arrange

        var validQuizName = "Valid Quiz Name";

        // Act

        var result = _isValidQuizNameValidator.TestValidate(validQuizName);

        // Assert

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void IsValidQuizName_WhenTooLongQuizName_ShouldHaveValidationError()
    {
        // Arrange

        var tooLongQuizName = new string('A', 129);

        // Act

        var result = _isValidQuizNameValidator.TestValidate(tooLongQuizName);

        // Assert

        result.ShouldHaveValidationErrorFor(x => x)
            .WithErrorMessage("Quiz name must be at most 128 characters long");
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void IsValidQuizName_WhenEmptyOrWhitespaceQuizName_ShouldHaveValidationError(string quizName)
    {
        // Act

        var result = _isValidQuizNameValidator.TestValidate(quizName);

        // Assert

        result.ShouldHaveValidationErrorFor(x => x)
            .WithErrorMessage("Quiz name cannot be empty or whitespace");
    }

    #endregion

    #region Test Quiz Description

    [Fact]
    public void IsValidQuizDescription_WhenValidQuizDescription_ShouldNotHaveValidationErrors()
    {
        // Arrange

        var validQuizDescription = "Valid Quiz Description";

        // Act

        var result = _isValidQuizDescriptionValidator.TestValidate(validQuizDescription);

        // Assert

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void IsValidQuizDescription_WhenTooLongQuizDescription_ShouldHaveValidationError()
    {
        // Arrange

        var tooLongQuizDescription = new string('A', 513);

        // Act

        var result = _isValidQuizDescriptionValidator.TestValidate(tooLongQuizDescription);

        // Assert

        result.ShouldHaveValidationErrorFor(x => x)
            .WithErrorMessage("Quiz description must be at most 512 characters long");
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void IsValidQuizDescription_WhenEmptyOrWhitespaceQuizDescription_ShouldHaveValidationError(string quizDescription)
    {
        // Act

        var result = _isValidQuizDescriptionValidator.TestValidate(quizDescription);

        // Assert

        result.ShouldHaveValidationErrorFor(x => x)
            .WithErrorMessage("Quiz description cannot be empty or whitespace");
    }

    #endregion

    #region Test Quiz Categories

    [Theory]
    [InlineData((object)new string[] { "test" })]
    [InlineData((object)new string[] { "test 1", "test 2" })]
    [InlineData((object)new string[] { "1", "2", "3", "4", "5", "6", "7", "8", "9", "10" })]
    public void IsValidQuizCategories_WhenValidQuizCategories_ShouldNotHaveValidationErrors(ICollection<string> quizCategories)
    {
        // Act

        var result = _isValidQuizCategoriesValidator.TestValidate(quizCategories);

        // Assert

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void IsValidQuizCategories_WhenEmptyQuizCategories_ShouldHaveValidationError()
    {
        // Arrange

        var emptyQuizCategories = Array.Empty<string>();

        // Act

        var result = _isValidQuizCategoriesValidator.TestValidate(emptyQuizCategories);

        // Assert

        result.ShouldHaveValidationErrorFor(x => x)
            .WithErrorMessage("At least one category is required");
    }

    [Fact]
    public void IsValidQuizCategories_WhenTooManyQuizCategories_ShouldHaveValidationError()
    {
        // Arrange

        var tooManyQuizCategories = new string[] { "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11" };

        // Act

        var result = _isValidQuizCategoriesValidator.TestValidate(tooManyQuizCategories);

        // Assert

        result.ShouldHaveValidationErrorFor(x => x)
            .WithErrorMessage("You can specify up to 10 categories");
    }

    [Theory]
    [InlineData((object)new string[] { "" } )]
    [InlineData((object)new string[] { " " })]
    [InlineData((object)new string[] { "test", "" })]
    [InlineData((object)new string[] { "test", " " })]
    public void IsValidQuizCategories_WhenEmptyOrWhitespaceAnyQuizCategory_ShouldHaveValidationError(ICollection<string> quizCategories)
    {
        // Act

        var result = _isValidQuizCategoriesValidator.TestValidate(quizCategories);

        // Assert

        result.ShouldHaveValidationErrorFor(x => x)
            .WithErrorMessage("Category cannot be empty or whitespace");
    }

    [Theory]
    [InlineData((object)new string[] { "test", "test" })]
    [InlineData((object)new string[] { "test", "TEST" })]
    [InlineData((object)new string[] { "test", "TeSt" })]
    [InlineData((object)new string[] { "test", " t e s t " })]
    [InlineData((object)new string[] { "test", "test 2", "test" })]
    public void IsValidQuizCategories_WhenNotUniqueQuizCategoriesRegardlessOfCaseAndSpaces_ShouldHaveValidationError(ICollection<string> quizCategories)
    {
        // Act

        var result = _isValidQuizCategoriesValidator.TestValidate(quizCategories);

        // Assert

        result.ShouldHaveValidationErrorFor(x => x)
            .WithErrorMessage("Categories must be unique");
    }

    [Theory]
    [InlineData((object)new string[] { })]
    [InlineData((object)new string[] { "test" })]
    public void IsValidQuizCategories_WhenTooLongAnyQuizCategory_ShouldHaveValidationError(ICollection<string> quizCategories)
    {
        // Arrange

        var quizCategoriesList = quizCategories.ToList();
        quizCategoriesList.Add(new string('A', 65));

        // Act

        var result = _isValidQuizCategoriesValidator.TestValidate(quizCategoriesList);

        // Assert

        result.ShouldHaveValidationErrorFor(x => x)
            .WithErrorMessage("Each category must not exceed 64 characters");
    }

    #endregion
}

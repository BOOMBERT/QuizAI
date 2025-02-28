using FluentValidation;
using FluentValidation.TestHelper;
using Xunit;

namespace QuizAI.Application.Extensions.Tests;

public class QuizValidationExtensionsTests
{
    #region Test Quiz Name

    private InlineValidator<string> GetIsValidQuizNameValidator()
    {
        var validator = new InlineValidator<string>();
        validator.RuleFor(x => x).IsValidQuizName();

        return validator;
    }

    [Fact]
    public void IsValidQuizName_WhenValidQuizName_ShouldNotHaveValidationErrors()
    {
        // Arrange

        var validQuizName = "Valid Quiz Name";

        // Act

        var result = GetIsValidQuizNameValidator().TestValidate(validQuizName);

        // Assert

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void IsValidQuizName_WhenTooLongQuizName_ShouldHaveValidationError()
    {
        // Arrange

        var tooLongQuizName = new string('A', 129);

        // Act

        var result = GetIsValidQuizNameValidator().TestValidate(tooLongQuizName);

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

        var result = GetIsValidQuizNameValidator().TestValidate(quizName);

        // Assert

        result.ShouldHaveValidationErrorFor(x => x)
            .WithErrorMessage("Quiz name cannot be empty or whitespace");
    }

    #endregion

    #region Test Quiz Description

    private InlineValidator<string?> GetIsValidQuizDescriptionValidator()
    {
        var validator = new InlineValidator<string?>();
        validator.RuleFor(x => x).IsValidQuizDescription();

        return validator;
    }

    [Fact]
    public void IsValidQuizDescription_WhenValidQuizDescription_ShouldNotHaveValidationErrors()
    {
        // Arrange

        var validQuizDescription = "Valid Quiz Description";

        // Act

        var result = GetIsValidQuizDescriptionValidator().TestValidate(validQuizDescription);

        // Assert

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void IsValidQuizDescription_WhenTooLongQuizDescription_ShouldHaveValidationError()
    {
        // Arrange

        var tooLongQuizDescription = new string('A', 513);

        // Act

        var result = GetIsValidQuizDescriptionValidator().TestValidate(tooLongQuizDescription);

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

        var result = GetIsValidQuizDescriptionValidator().TestValidate(quizDescription);

        // Assert

        result.ShouldHaveValidationErrorFor(x => x)
            .WithErrorMessage("Quiz description cannot be empty or whitespace");
    }

    #endregion

    #region Test Quiz Categories

    private InlineValidator<ICollection<string>> GetIsValidQuizCategoriesValidator()
    {
        var validator = new InlineValidator<ICollection<string>>();
        validator.RuleFor(x => x).IsValidQuizCategories();

        return validator;
    }

    [Theory]
    [InlineData((object)new string[] { "test" })]
    [InlineData((object)new string[] { "test 1", "test 2" })]
    [InlineData((object)new string[] { "1", "2", "3", "4", "5", "6", "7", "8", "9", "10" })]
    public void IsValidQuizCategories_WhenValidQuizCategories_ShouldNotHaveValidationErrors(ICollection<string> quizCategories)
    {
        // Act

        var result = GetIsValidQuizCategoriesValidator().TestValidate(quizCategories);

        // Assert

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void IsValidQuizCategories_WhenEmptyQuizCategories_ShouldHaveValidationError()
    {
        // Arrange

        var emptyQuizCategories = Array.Empty<string>();

        // Act

        var result = GetIsValidQuizCategoriesValidator().TestValidate(emptyQuizCategories);

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

        var result = GetIsValidQuizCategoriesValidator().TestValidate(tooManyQuizCategories);

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

        var result = GetIsValidQuizCategoriesValidator().TestValidate(quizCategories);

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

        var result = GetIsValidQuizCategoriesValidator().TestValidate(quizCategories);

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

        var result = GetIsValidQuizCategoriesValidator().TestValidate(quizCategoriesList);

        // Assert

        result.ShouldHaveValidationErrorFor(x => x)
            .WithErrorMessage("Each category must not exceed 64 characters");
    }

    #endregion
}

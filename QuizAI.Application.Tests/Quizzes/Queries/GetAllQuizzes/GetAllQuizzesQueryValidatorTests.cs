using FluentValidation.TestHelper;
using Xunit;

namespace QuizAI.Application.Quizzes.Queries.GetAllQuizzes.Tests;

public class GetAllQuizzesQueryValidatorTests
{
    private (GetAllQuizzesQuery, GetAllQuizzesQueryValidator) GetAllQuizzesQueryAndValidator(string[] filterByCategories)
        => (new GetAllQuizzesQuery() { FilterByCategories = filterByCategories, PageNumber = 1, PageSize = 5 }, new GetAllQuizzesQueryValidator());

    [Theory]
    [InlineData((object)new string[] { "test" })]
    [InlineData((object)new string[] { "test", "test 2" })]
    [InlineData((object)new string[0])]
    public void Validator_WhenValidQuery_ShouldNotHaveValidationErrors(string[] validCategoriesToFilter)
    {
        // Arrange

        var (query, validator) = GetAllQuizzesQueryAndValidator(validCategoriesToFilter);
    
        // Act

        var result = validator.TestValidate(query);

        // Assert

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validator_WhenTooManyCategoriesToFilter_ShouldHaveValidationError()
    {
        // Arrange

        var (query, validator) = GetAllQuizzesQueryAndValidator(new string[] { "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11"});

        // Act

        var result = validator.TestValidate(query);

        // Assert

        result.ShouldHaveValidationErrorFor(x => x.FilterByCategories)
            .WithErrorMessage("You can specify up to 10 categories");
    }

    [Theory]
    [InlineData((object)new string[] { "test", "test" })]
    [InlineData((object)new string[] { "test", "Test" })]
    public void Validator_WhenNotUniqueCategoriesToFilter_ShouldHaveValidationError(string[] categoriesToFilter)
    {
        // Arrange

        var (query, validator) = GetAllQuizzesQueryAndValidator(categoriesToFilter);

        // Act

        var result = validator.TestValidate(query);

        // Assert

        result.ShouldHaveValidationErrorFor(x => x.FilterByCategories)
            .WithErrorMessage("Categories must be unique");
    }

    [Theory]
    [InlineData((object)new string[] { ""})]
    [InlineData((object)new string[] { " "})]
    [InlineData((object)new string[] { "test", "" })]
    [InlineData((object)new string[] { "test", " " })]
    public void Validator_WhenEmptyOrWhitespaceAnyCategoryToFilter_ShouldHaveValidationError(string[] categoriesToFilter)
    {
        // Arrange

        var (query, validator) = GetAllQuizzesQueryAndValidator(categoriesToFilter);

        // Act

        var result = validator.TestValidate(query);

        // Assert

        result.ShouldHaveValidationErrorFor(x => x.FilterByCategories)
            .WithErrorMessage("Category cannot be empty or whitespace");
    }

    [Theory]
    [InlineData((object)new string[0])]
    [InlineData((object)new string[] { "test" })]
    public void Validator_WhenTooLongAnyCategoryToFilter_ShouldHaveValidationError(string[] categoriesToFilter)
    {
        // Arrange

        var categoriesToFilterList = categoriesToFilter.ToList();
        categoriesToFilterList.Add(new string('A', 65));

        var (query, validator) = GetAllQuizzesQueryAndValidator(categoriesToFilterList.ToArray());

        // Act

        var result = validator.TestValidate(query);

        // Assert

        result.ShouldHaveValidationErrorFor(x => x.FilterByCategories)
            .WithErrorMessage("Each category must not exceed 64 characters");
    }
}

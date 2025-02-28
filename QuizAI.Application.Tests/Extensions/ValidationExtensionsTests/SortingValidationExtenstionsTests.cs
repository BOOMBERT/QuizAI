using FluentValidation;
using FluentValidation.TestHelper;
using QuizAI.Application.Interfaces;
using QuizAI.Application.Tests.TestHelpers;
using QuizAI.Domain.Constants;
using Xunit;

namespace QuizAI.Application.Extensions.Tests;

public class SortingValidationExtensionsTests
{
    #region Test Sort Direction

    private InlineValidator<IPaginationQuery> GetIsValidSortDirectionValidator()
    {
        var validator = new InlineValidator<IPaginationQuery>();
        validator.RuleFor(x => x.SortDirection).IsValidSortDirection();

        return validator;
    }

    [Fact]
    public void IsValidSortDirection_WhenNotNullSortDirectionAndNotNullOrEmptySortBy_ShouldNotHaveValidationErrors()
    {
        // Arrange

        var paginationQuery = new PaginationQuery() { SortDirection = SortDirection.Ascending, SortBy = "test" };

        // Act

        var result = GetIsValidSortDirectionValidator().TestValidate(paginationQuery);

        // Assert

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void IsValidSortDirection_WhenNullSortDirectionAndNullOrEmptySortBy_ShouldNotHaveValidationErrors(string? sortBy)
    {
        // Arrange

        var paginationQuery = new PaginationQuery() { SortDirection = null, SortBy = sortBy };

        // Act

        var result = GetIsValidSortDirectionValidator().TestValidate(paginationQuery);

        // Assert

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void IsValidSortDirection_WhenNullSortDirectionAndNotNullOrEmptySortBy_ShouldHaveValidationError()
    {
        // Arrange

        var paginationQuery = new PaginationQuery() { SortDirection = null, SortBy = "test" };

        // Act

        var result = GetIsValidSortDirectionValidator().TestValidate(paginationQuery);

        // Assert

        result.ShouldHaveValidationErrorFor(x => x.SortDirection)
            .WithErrorMessage("Sort direction is required when Sort by is provided and must be empty when Sort by is empty");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void IsValidSortDirection_WhenNotNullSortDirectionAndNullOrEmptySortBy_ShouldHaveValidationError(string? sortBy)
    {
        // Arrange

        var paginationQuery = new PaginationQuery() { SortDirection = SortDirection.Descending, SortBy = sortBy };

        // Act

        var result = GetIsValidSortDirectionValidator().TestValidate(paginationQuery);

        // Assert

        result.ShouldHaveValidationErrorFor(x => x.SortDirection)
            .WithErrorMessage("Sort direction is required when Sort by is provided and must be empty when Sort by is empty");
    }

    #endregion

    #region Test Sort By

    private readonly string[] allowedSortByColumnNames = ["test", "test 2"];

    private InlineValidator<IPaginationQuery> GetIsValidSortByValidator()
    {
        var validator = new InlineValidator<IPaginationQuery>();
        validator.RuleFor(x => x.SortBy).IsValidSortBy(allowedSortByColumnNames);

        return validator;
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("test")]
    [InlineData("Test")]
    [InlineData("test 2")]
    public void IsValidSortBy_WhenValidSortBy_ShouldNotHaveValidationErrors(string? sortBy)
    {
        // Arrange

        var paginationQuery = new PaginationQuery() { SortBy = sortBy };

        // Act

        var result = GetIsValidSortByValidator().TestValidate(paginationQuery);

        // Assert

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData("test 3")]
    [InlineData("te st")]
    public void IsValidSortBy_WhenInvalidSortBy_ShouldHaveValidationError(string? sortBy)
    {
        // Arrange

        var paginationQuery = new PaginationQuery() { SortBy = sortBy };

        // Act

        var result = GetIsValidSortByValidator().TestValidate(paginationQuery);

        // Assert

        result.ShouldHaveValidationErrorFor(x => x.SortBy)
            .WithErrorMessage($"Sort by is optional, or must be in [{string.Join(",", allowedSortByColumnNames)}]");
    }

    #endregion
}

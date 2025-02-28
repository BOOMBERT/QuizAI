using FluentValidation;
using FluentValidation.TestHelper;
using QuizAI.Application.Interfaces;
using QuizAI.Application.Tests.TestHelpers;
using Xunit;

namespace QuizAI.Application.Extensions.Tests;

public class PaginationValidationExtensionsTests
{
    #region Test Page Number

    private InlineValidator<IPaginationQuery> GetIsValidPageNumberValidator()
    {
        var validator = new InlineValidator<IPaginationQuery>();
        validator.RuleFor(x => x.PageNumber).IsValidPageNumber();

        return validator;
    }

    [Theory]
    [InlineData(1)]
    [InlineData(10)]
    public void IsValidPageNumber_WhenValidPageNumber_ShouldNotHaveValidationErrors(int pageNumber)
    {
        // Arrange

        var paginationQuery = new PaginationQuery() { PageNumber = pageNumber };

        // Act

        var result = GetIsValidPageNumberValidator().TestValidate(paginationQuery);

        // Assert

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void IsValidPageNumber_WhenInvalidPageNumber_ShouldHaveValidationError(int pageNumber)
    {
        // Arrange

        var paginationQuery = new PaginationQuery() { PageNumber = pageNumber };

        // Act

        var result = GetIsValidPageNumberValidator().TestValidate(paginationQuery);

        // Assert

        result.ShouldHaveValidationErrorFor(x => x.PageNumber)
            .WithErrorMessage("Page number must be greater than or equal to 1");
    }

    #endregion

    #region Test Page Size

    private readonly int[] allowPageSizes = [5, 10, 15, 30];

    private InlineValidator<IPaginationQuery> GetIsValidPageSizeValidator()
    {
        var validator = new InlineValidator<IPaginationQuery>();
        validator.RuleFor(x => x.PageSize).IsValidPageSize(allowPageSizes);

        return validator;
    }

    [Theory]
    [InlineData(5)]
    [InlineData(15)]
    public void IsValidPageSize_WhenValidPageSize_ShouldNotHaveValidationErrors(int pageSize)
    {
        // Arrange

        var paginationQuery = new PaginationQuery() { PageSize = pageSize };

        // Act

        var result = GetIsValidPageSizeValidator().TestValidate(paginationQuery);

        // Assert

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData(1)]
    [InlineData(12)]
    [InlineData(31)]
    public void IsValidPageSize_WhenInvalidPageSize_ShouldHaveValidationError(int pageSize)
    {
        // Arrange

        var paginationQuery = new PaginationQuery() { PageSize = pageSize };

        // Act

        var result = GetIsValidPageSizeValidator().TestValidate(paginationQuery);

        // Assert

        result.ShouldHaveValidationErrorFor(x => x.PageSize)
            .WithErrorMessage($"Page size must be in [{string.Join(",", allowPageSizes)}]");
    }

    #endregion
}

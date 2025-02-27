using FluentValidation;
using FluentValidation.TestHelper;
using QuizAI.Application.Interfaces;
using QuizAI.Domain.Constants;
using Xunit;

namespace QuizAI.Application.Extensions.Tests;

public class PaginationValidationExtensionsTests
{
    private readonly InlineValidator<IPaginationQuery> _IsValidPageNumberValidator;
    private readonly InlineValidator<IPaginationQuery> _IsValidPageSizeValidator;

    private readonly int[] allowPageSizes = [5, 10, 15, 30];

    public PaginationValidationExtensionsTests()
    {
        _IsValidPageNumberValidator = new InlineValidator<IPaginationQuery>();
        _IsValidPageNumberValidator.RuleFor(x => x.PageNumber).IsValidPageNumber();

        _IsValidPageSizeValidator = new InlineValidator<IPaginationQuery>();
        _IsValidPageSizeValidator.RuleFor(x => x.PageSize).IsValidPageSize(allowPageSizes);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(10)]
    public void IsValidPageNumber_WhenValidPageNumber_ShouldNotHaveValidationErrors(int pageNumber)
    {
        // Arrange

        var paginationQuery = new PaginationQuery() { PageNumber = pageNumber };

        // Act

        var result = _IsValidPageNumberValidator.TestValidate(paginationQuery);

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

        var result = _IsValidPageNumberValidator.TestValidate(paginationQuery);

        // Assert

        result.ShouldHaveValidationErrorFor(x => x.PageNumber)
            .WithErrorMessage("Page number must be greater than or equal to 1");
    }

    [Theory]
    [InlineData(5)]
    [InlineData(15)]
    public void IsValidPageSize_WhenValidPageSize_ShouldNotHaveValidationErrors(int pageSize)
    {
        // Arrange

        var paginationQuery = new PaginationQuery() { PageSize = pageSize };

        // Act

        var result = _IsValidPageSizeValidator.TestValidate(paginationQuery);

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

        var result = _IsValidPageSizeValidator.TestValidate(paginationQuery);

        // Assert

        result.ShouldHaveValidationErrorFor(x => x.PageSize)
            .WithErrorMessage($"Page size must be in [{string.Join(",", allowPageSizes)}]");
    }
}

internal class PaginationQuery : IPaginationQuery
{
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public string? SortBy { get; set; }
    public SortDirection? SortDirection { get; set; }
}

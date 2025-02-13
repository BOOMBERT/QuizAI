using FluentValidation;
using QuizAI.Application.Extensions;
using QuizAI.Application.QuizAttempts.Dtos;
using QuizAI.Application.QuizAttempts.Queries.GetAllAttempts;
using QuizAI.Domain.Entities;

namespace QuizAI.Application.QuizAttempts.Queries.GetAllAtempts;

public class GetAllAttemptsQueryValidator : AbstractValidator<GetAllAttemptsQuery>
{
    private int[] allowPageSizes = [5, 10, 15, 30];
    private string[] allowedSortByColumnNames = [
        nameof(QuizAttempt.StartedAt),
        nameof(QuizAttempt.FinishedAt)
    ];

    public GetAllAttemptsQueryValidator()
    {
        RuleFor(qa => qa.PageNumber)
            .IsValidPageNumber();

        RuleFor(qa => qa.PageSize)
            .IsValidPageSize(allowPageSizes);

        RuleFor(qa => qa.SortDirection)
            .IsValidSortDirection();

        RuleFor(qa => qa.SortBy)
            .IsValidSortBy(allowedSortByColumnNames);

        RuleFor(qa => qa.SearchPhrase)
            .MaximumLength(128).WithMessage("Search phrase must be at most 128 characters long");

        RuleFor(qa => qa.FilterStartedAtYearAndMonth)
            .IsValidUtcDateTime();

        RuleFor(qa => qa.FilterStartedAtYearAndMonth)
            .IsValidUtcDateTime();
    }
}

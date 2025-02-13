using FluentValidation;
using QuizAI.Application.Extensions;
using QuizAI.Domain.Entities;

namespace QuizAI.Application.Quizzes.Queries.GetAllQuizzes;

public class GetAllQuizzesQueryValidator : AbstractValidator<GetAllQuizzesQuery>
{
    private int[] allowPageSizes = [5, 10, 15, 30];
    private string[] allowedSortByColumnNames = [
        nameof(Quiz.Name),
        nameof(Quiz.Description), 
        nameof(Quiz.CreationDate)
    ];

    public GetAllQuizzesQueryValidator()
    {
        RuleFor(qz => qz.PageNumber)
            .IsValidPageNumber();

        RuleFor(qz => qz.PageSize)
            .IsValidPageSize(allowPageSizes);

        RuleFor(qz => qz.SortDirection)
            .IsValidSortDirection();

        RuleFor(qz => qz.SortBy)
            .IsValidSortBy(allowedSortByColumnNames);

        RuleFor(qz => qz.SearchPhrase)
            .MaximumLength(512).WithMessage("Search phrase must be at most 512 characters long");

        RuleFor(qz => qz.FilterCategories)
            .Must(fc => fc.Count <= 10).WithMessage("You can specify up to 10 categories")
            .Must(fc => fc.Select(fc => fc.ToLower()).ToHashSet().Count == fc.Count).WithMessage("Categories must be unique")
            .Must(fc => fc.All(fc => !string.IsNullOrWhiteSpace(fc))).WithMessage("Category cannot be empty or whitespace")
            .Must(fc => fc.All(fc => fc.Length <= 64)).WithMessage("Each category must not exceed 64 characters");
    }
}

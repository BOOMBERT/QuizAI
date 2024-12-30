using FluentValidation;
using QuizAI.Application.Quizzes.Dtos;

namespace QuizAI.Application.Quizzes.Queries.GetAllQuizzes;

public class GetAllQuizzesQueryValidator : AbstractValidator<GetAllQuizzesQuery>
{
    private int[] allowPageSizes = [5, 10, 15, 30];
    private string[] allowedSortByColumnNames = [
        nameof(QuizDto.Name),
        nameof(QuizDto.Description), 
        nameof(QuizDto.CreationDate)
    ];

    public GetAllQuizzesQueryValidator()
    {
        RuleFor(qz => qz.SearchPhrase)
            .MaximumLength(512).WithMessage("Search phrase must be at most 512 characters long.");

        RuleFor(qz => qz.PageNumber)
            .GreaterThanOrEqualTo(1).WithMessage("Page number must be greater than or equal to 1.");

        RuleFor(qz => qz.PageSize)
            .Must(ps => allowPageSizes.Contains(ps))
            .WithMessage($"Page size must be in [{string.Join(",", allowPageSizes)}]");

        RuleFor(qz => qz.SortDirection)
            .NotNull()
            .WithMessage("Sort direction is required when Sort by is provided.")
            .When(qz => qz.SortBy != null);

        RuleFor(qz => qz.SortBy)
            .NotNull()
            .WithMessage("Sort by is required when Sort direction is provided.")
            .When(qz => qz.SortDirection != null);

        RuleFor(qz => qz.SortBy)
            .Must(sb => allowedSortByColumnNames.Any(asbcn => string.Equals(asbcn, sb, StringComparison.OrdinalIgnoreCase)))
            .When(qz => qz.SortBy != null)
            .WithMessage($"Sort by is optional, or must be in [{string.Join(",", allowedSortByColumnNames)}].");

        RuleFor(qz => qz.FilterCategories)
            .Must(fc => fc.Count <= 10).WithMessage("You can specify up to 10 categories.")
            .Must(fc => fc.Select(fc => fc.ToLower()).ToHashSet().Count == fc.Count).WithMessage("Categories must be unique.")
            .Must(fc => fc.All(fc => !string.IsNullOrWhiteSpace(fc))).WithMessage("Category cannot be empty or whitespace.")
            .Must(fc => fc.All(fc => fc.Length <= 64)).WithMessage("Each category must not exceed 64 characters.");
    }
}

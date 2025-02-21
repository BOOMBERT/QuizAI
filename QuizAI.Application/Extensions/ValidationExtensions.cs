using FluentValidation;
using Microsoft.AspNetCore.Http;
using QuizAI.Application.Interfaces;
using QuizAI.Application.MultipleChoiceQuestions.Dtos;
using QuizAI.Application.Utils;
using QuizAI.Domain.Constants;

namespace QuizAI.Application.Extensions;

internal static class ValidationExtensions
{
    private const int MaxImageSizeInBytes = 2 * 1024 * 1024; // 2MB

    internal static IRuleBuilderOptions<T, string> IsValidQuizName<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder.MaximumLength(128).WithMessage("Quiz name must be at most 128 characters long");
    }

    internal static IRuleBuilderOptions<T, string?> IsValidQuizDescription<T>(this IRuleBuilder<T, string?> ruleBuilder)
    {
        return ruleBuilder.MaximumLength(512).WithMessage("Quiz description must be at most 512 characters long");
    }

    internal static IRuleBuilderOptions<T, ICollection<string>> IsValidQuizCategories<T>(this IRuleBuilder<T, ICollection<string>> ruleBuilder)
    {
        return ruleBuilder
            .NotEmpty().WithMessage("At least one category is required")
            .Must(c => c.Count <= 10).WithMessage("You can specify up to 10 categories")
            .Must(c => c.All(c => !string.IsNullOrWhiteSpace(c))).WithMessage("Category cannot be empty or whitespace")
            .Must(c => c.Select(c => c?.Trim().ToLower()).ToHashSet().Count == c.Count).WithMessage("Categories must be unique")
            .Must(c => c.All(c => c == null || c.Length <= 64)).WithMessage("Each category must not exceed 64 characters");
    }
    
    internal static IRuleBuilderOptions<T, string> IsValidQuestionContent<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder
            .Must(qc => !string.IsNullOrWhiteSpace(qc)).WithMessage("Question content cannot be empty or whitespace")
            .MaximumLength(512).WithMessage("Question content must be at most 512 characters long");
    }

    internal static IRuleBuilderOptions<T, ICollection<MultipleChoiceAnswerDto>> IsValidMultipleChoiceAnswers<T>(
        this IRuleBuilder<T, ICollection<MultipleChoiceAnswerDto>> ruleBuilder)
    {
        return ruleBuilder
            .Must(a => a.Count >= 2).WithMessage("At least two answers are required")
            .Must(a => a.Count <= 8).WithMessage("You can specify up to 8 answers")
            .Must(a => a.Select(a => a.Content).ToHashSet().Count == a.Count).WithMessage("Answers must be unique")
            .Must(a => a.All(a => !string.IsNullOrWhiteSpace(a.Content))).WithMessage("Answer cannot be empty or whitespace")
            .Must(a => a.All(a => a.Content.Length <= 255)).WithMessage("Each answer content must not exceed 255 characters");
    }

    internal static IRuleBuilderOptions<T, ICollection<string>> IsValidOpenEndedAnswers<T>(this IRuleBuilder<T, ICollection<string>> ruleBuilder)
    {
        return ruleBuilder
            .Must(a => a.Count >= 1).WithMessage("At least one answer is required")
            .Must(a => a.Count <= 20).WithMessage("You can specify up to 20 answers")
            .Must(a => a.ToHashSet().Count == a.Count).WithMessage("Answers must be unique")
            .Must(a => a.All(a => !string.IsNullOrWhiteSpace(a))).WithMessage("Answer cannot be empty or whitespace")
            .Must(a => a.Sum(a => a.Length) <= 1275).WithMessage("The total length of all answers must not exceed 1275 characters");
    }

    internal static IRuleBuilderOptions<T, IFormFile?> IsValidImage<T>(this IRuleBuilder<T, IFormFile?> ruleBuilder)
    {
        return (IRuleBuilderOptions<T, IFormFile>)ruleBuilder.Custom((image, context) =>
        {
            if (image != null)
                FileValidationUtil.Validate(image, MaxImageSizeInBytes);
        });
    }

    internal static IRuleBuilder<T, DateTime?> IsValidUtcDateTime<T>(this IRuleBuilder<T, DateTime?> ruleBuilder)
    {
        return ruleBuilder
            .Must(d => !d.HasValue || d.Value <= DateTime.UtcNow)
            .WithMessage("The date cannot exceed the current date");
    }

    internal static IRuleBuilderOptions<T, int> IsValidPageNumber<T>(this IRuleBuilder<T, int> ruleBuilder) 
        where T : IPaginationQuery
    {
        return ruleBuilder
            .GreaterThanOrEqualTo(1).WithMessage("Page number must be greater than or equal to 1");
    }

    internal static IRuleBuilderOptions<T, int> IsValidPageSize<T>(this IRuleBuilder<T, int> ruleBuilder, int[] allowPageSizes) 
        where T : IPaginationQuery
    {
        return ruleBuilder
            .Must(ps => allowPageSizes.Contains(ps)).WithMessage($"Page size must be in [{string.Join(",", allowPageSizes)}]");
    }

    internal static IRuleBuilderOptions<T, SortDirection?> IsValidSortDirection<T>(this IRuleBuilder<T, SortDirection?> ruleBuilder) 
        where T : IPaginationQuery
    {
        return ruleBuilder
               .Must((instance, sortDirection) =>
                   (!string.IsNullOrEmpty(instance.SortBy) && sortDirection != null) ||
                   (string.IsNullOrEmpty(instance.SortBy) && sortDirection == null)
               )
               .WithMessage("Sort direction is required when Sort by is provided and must be empty when Sort by is empty");
    }

    internal static IRuleBuilderOptions<T, string?> IsValidSortBy<T>(this IRuleBuilder<T, string?> ruleBuilder, string[] allowedSortByColumnNames) 
        where T : IPaginationQuery
    {
        return ruleBuilder
            .Must(sb => allowedSortByColumnNames.Any(asbcn => string.Equals(asbcn, sb, StringComparison.OrdinalIgnoreCase)))
            .WithMessage($"Sort by is optional, or must be in [{string.Join(",", allowedSortByColumnNames)}]")
            .When(f => !string.IsNullOrEmpty(f.SortBy));
    }
}

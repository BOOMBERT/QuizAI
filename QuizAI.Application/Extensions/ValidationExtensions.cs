using FluentValidation;
using Microsoft.AspNetCore.Http;
using QuizAI.Application.MultipleChoiceQuestions.Dtos;
using QuizAI.Application.Utils;

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
            .MaximumLength(255).WithMessage("Question content must be at most 255 characters long");
    }

    internal static IRuleBuilderOptions<T, ICollection<CreateMultipleChoiceAnswerDto>> IsValidMultipleChoiceAnswers<T>(
        this IRuleBuilder<T, ICollection<CreateMultipleChoiceAnswerDto>> ruleBuilder)
    {
        return ruleBuilder
            .Must(a => a.Count >= 2).WithMessage("At least two answers are required")
            .Must(a => a.Count <= 8).WithMessage("You can specify up to 8 answers")
            .Must(a => a.Select(a => a.Content.Trim().ToLower()).ToHashSet().Count == a.Count).WithMessage("Answers must be unique")
            .Must(a => a.All(a => !string.IsNullOrEmpty(a.Content))).WithMessage("Answer cannot be empty or whitespace")
            .Must(a => a.All(a => a.Content.Length <= 255)).WithMessage("Each answer content must not exceed 255 characters");
    }

    internal static IRuleBuilderOptions<T, ICollection<string>> IsValidOpenEndedAnswers<T>(this IRuleBuilder<T, ICollection<string>> ruleBuilder)
    {
        return ruleBuilder
            .Must(a => a.Count <= 5).WithMessage("You can specify up to 5 answers")
            .Must(a => a.ToHashSet().Count == a.Count).WithMessage("Answers must be unique")
            .Must(a => a.All(a => !string.IsNullOrEmpty(a))).WithMessage("Answer cannot be empty or whitespace")
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
}

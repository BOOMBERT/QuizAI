using FluentValidation;

namespace QuizAI.Application.Quizzes.Commands.UpdateQuiz;

public class UpdateQuizCommandValidator : AbstractValidator<UpdateQuizCommand>
{
    public UpdateQuizCommandValidator()
    {
        RuleFor(qz => qz.Name)
            .MaximumLength(128).WithMessage("Quiz name must be at most 128 characters long.");

        RuleFor(qz => qz.Description)
            .MaximumLength(512).WithMessage("Quiz description must be at most 512 characters long.");

        RuleFor(qz => qz.Categories)
            .NotEmpty().WithMessage("At least one category is required.")
            .Must(c => c.Count <= 10).WithMessage("You can specify up to 10 categories.")
            .Must(c => c.ToHashSet().Count == c.Count).WithMessage("Categories must be unique.");

        RuleForEach(qz => qz.Categories)
            .Must(c => !string.IsNullOrWhiteSpace(c)).WithMessage("Category cannot be empty or whitespace.")
            .Must(c => c == null || c.Length <= 64).WithMessage("Each category must not exceed 64 characters.");
    }
}

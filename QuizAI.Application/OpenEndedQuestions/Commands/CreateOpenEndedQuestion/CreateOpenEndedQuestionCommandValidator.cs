using FluentValidation;

namespace QuizAI.Application.OpenEndedQuestions.Commands.CreateOpenEndedQuestion;

public class CreateOpenEndedQuestionCommandValidator : AbstractValidator<CreateOpenEndedQuestionCommand>
{
    public CreateOpenEndedQuestionCommandValidator()
    {
        RuleFor(oeq => oeq.Content)
            .MaximumLength(255).WithMessage("Question content must be at most 255 characters long.");

        RuleFor(oeq => oeq.VerificationByAI)
            .NotEqual(false)
            .WithMessage("Verification by AI must be enabled when answers are empty.")
            .When(oeq => oeq.Answers.Count == 0);

        RuleFor(oeq => oeq.Answers)
            .Must(a => a.Count <= 5).WithMessage("You can specify up to 5 answers.")
            .Must(a => a.Sum(a => a.Length) <= 1275).WithMessage("The total length of all answers must not exceed 1275 characters.");

        RuleForEach(oeq => oeq.Answers)
            .Must(a => !string.IsNullOrWhiteSpace(a)).WithMessage("Answer cannot be empty or whitespace.");
    }
}

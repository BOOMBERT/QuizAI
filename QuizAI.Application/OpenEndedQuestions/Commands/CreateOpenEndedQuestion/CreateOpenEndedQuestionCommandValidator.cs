using FluentValidation;
using QuizAI.Application.Extensions;

namespace QuizAI.Application.OpenEndedQuestions.Commands.CreateOpenEndedQuestion;

public class CreateOpenEndedQuestionCommandValidator : AbstractValidator<CreateOpenEndedQuestionCommand>
{
    public CreateOpenEndedQuestionCommandValidator()
    {
        RuleFor(oeq => oeq.Content)
            .IsValidQuestionContent();

        RuleFor(oeq => oeq.VerificationByAI)
            .NotEqual(false)
            .WithMessage("Verification by AI must be enabled when answers are empty.")
            .When(oeq => oeq.Answers.Count == 0);

        RuleFor(oeq => oeq.Answers)
            .IsValidOpenEndedAnswers();
    }
}

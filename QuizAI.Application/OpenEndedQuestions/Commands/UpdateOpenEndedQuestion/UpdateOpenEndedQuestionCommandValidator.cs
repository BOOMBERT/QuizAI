using FluentValidation;
using QuizAI.Application.Extensions;

namespace QuizAI.Application.OpenEndedQuestions.Commands.UpdateOpenEndedQuestion;

public class UpdateOpenEndedQuestionCommandValidator : AbstractValidator<UpdateOpenEndedQuestionCommand>
{
    public UpdateOpenEndedQuestionCommandValidator()
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

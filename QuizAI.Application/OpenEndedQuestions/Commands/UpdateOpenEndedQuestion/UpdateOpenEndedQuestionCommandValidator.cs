using FluentValidation;
using QuizAI.Application.Extensions;

namespace QuizAI.Application.OpenEndedQuestions.Commands.UpdateOpenEndedQuestion;

public class UpdateOpenEndedQuestionCommandValidator : AbstractValidator<UpdateOpenEndedQuestionCommand>
{
    public UpdateOpenEndedQuestionCommandValidator()
    {
        RuleFor(oeq => oeq.Content)
            .IsValidQuestionContent();

        RuleFor(oeq => oeq.Answers)
            .IsValidOpenEndedAnswers();
    }
}

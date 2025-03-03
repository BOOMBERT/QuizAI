using FluentValidation;
using QuizAI.Application.Extensions;

namespace QuizAI.Application.OpenEndedQuestions.Commands.CreateOpenEndedQuestion;

public class CreateOpenEndedQuestionCommandValidator : AbstractValidator<CreateOpenEndedQuestionCommand>
{
    public CreateOpenEndedQuestionCommandValidator()
    {
        RuleFor(oeq => oeq.Content)
            .IsValidQuestionContent();

        RuleFor(oeq => oeq.Answers)
            .IsValidOpenEndedAnswers();
    }
}

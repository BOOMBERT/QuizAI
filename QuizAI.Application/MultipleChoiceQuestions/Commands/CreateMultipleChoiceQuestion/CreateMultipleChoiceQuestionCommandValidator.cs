using FluentValidation;
using QuizAI.Application.Extensions;

namespace QuizAI.Application.MultipleChoiceQuestions.Commands.CreateMultipleChoiceQuestion;

public class CreateMultipleChoiceQuestionCommandValidator : AbstractValidator<CreateMultipleChoiceQuestionCommand>
{
    public CreateMultipleChoiceQuestionCommandValidator()
    {
        RuleFor(mcq => mcq.Content)
            .IsValidQuestionContent();

        RuleFor(mcq => mcq.Answers)
            .IsValidMultipleChoiceAnswers();
    }
}

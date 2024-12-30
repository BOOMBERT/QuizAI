using FluentValidation;
using QuizAI.Application.Extensions;

namespace QuizAI.Application.MultipleChoiceQuestions.Commands.UpdateMultipleChoiceQuestion;

public class UpdateMultipleChoiceQuestionCommandValidator : AbstractValidator<UpdateMultipleChoiceQuestionCommand>
{
    public UpdateMultipleChoiceQuestionCommandValidator()
    {
        RuleFor(mcq => mcq.Content)
            .IsValidQuestionContent();

        RuleFor(mcq => mcq.Answers)
            .IsValidMultipleChoiceAnswers();
    }
}

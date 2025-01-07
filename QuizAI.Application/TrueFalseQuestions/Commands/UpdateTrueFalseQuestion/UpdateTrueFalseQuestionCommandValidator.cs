using FluentValidation;
using QuizAI.Application.Extensions;

namespace QuizAI.Application.TrueFalseQuestions.Commands.UpdateTrueFalseQuestion;

public class UpdateTrueFalseQuestionCommandValidator : AbstractValidator<UpdateTrueFalseQuestionCommand>
{
    public UpdateTrueFalseQuestionCommandValidator()
    {
        RuleFor(tfq => tfq.Content)
            .IsValidQuestionContent();
    }
}

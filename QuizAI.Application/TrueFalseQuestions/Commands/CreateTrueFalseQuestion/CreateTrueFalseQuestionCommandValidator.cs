using FluentValidation;
using QuizAI.Application.Extensions;

namespace QuizAI.Application.TrueFalseQuestions.Commands.CreateTrueFalseQuestion;

public class CreateTrueFalseQuestionCommandValidator : AbstractValidator<CreateTrueFalseQuestionCommand>
{
    public CreateTrueFalseQuestionCommandValidator()
    {
        RuleFor(tfq => tfq.Content)
            .IsValidQuestionContent();
    }
}

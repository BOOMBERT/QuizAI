using FluentValidation;

namespace QuizAI.Application.TrueFalseQuestions.Commands.CreateTrueFalseQuestion;

public class CreateTrueFalseQuestionCommandValidator : AbstractValidator<CreateTrueFalseQuestionCommand>
{
    public CreateTrueFalseQuestionCommandValidator()
    {
        RuleFor(tfq => tfq.Content)
            .MaximumLength(255).WithMessage("Question content must be at most 255 characters long.");
    }
}

using FluentValidation;

namespace QuizAI.Application.MultipleChoiceQuestions.Commands.CreateMultipleChoiceQuestion;

public class CreateMultipleChoiceQuestionCommandValidator : AbstractValidator<CreateMultipleChoiceQuestionCommand>
{
    public CreateMultipleChoiceQuestionCommandValidator()
    {
        RuleFor(mcq => mcq.Content)
            .MaximumLength(255).WithMessage("Question content must be at most 255 characters long.");

        RuleFor(mcq => mcq.Answers)
            .Must(mcq => mcq.Count <= 8).WithMessage("You can specify up to 8 answers.");
    }
}

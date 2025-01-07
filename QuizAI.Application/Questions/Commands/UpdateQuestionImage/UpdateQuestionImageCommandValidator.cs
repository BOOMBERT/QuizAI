using FluentValidation;
using QuizAI.Application.Extensions;

namespace QuizAI.Application.Questions.Commands.UpdateQuestionImage;

public class UpdateQuestionImageCommandValidator : AbstractValidator<UpdateQuestionImageCommand>
{
    public UpdateQuestionImageCommandValidator()
    {
        RuleFor(qn => qn.Image)
            .IsValidImage();
    }
}

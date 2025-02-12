using FluentValidation;
using QuizAI.Application.Extensions;

namespace QuizAI.Application.QuestionImages.Commands.UpdateQuestionImage;

public class UpdateQuestionImageCommandValidator : AbstractValidator<UpdateQuestionImageCommand>
{
    public UpdateQuestionImageCommandValidator()
    {
        RuleFor(qn => qn.Image)
            .IsValidImage();
    }
}

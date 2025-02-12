using FluentValidation;
using QuizAI.Application.Extensions;

namespace QuizAI.Application.QuizImages.Commands.UpdateQuizImage;

public class UpdateQuizImageCommandValidator : AbstractValidator<UpdateQuizImageCommand>
{
    public UpdateQuizImageCommandValidator()
    {
        RuleFor(qz => qz.Image)
            .IsValidImage();
    }
}

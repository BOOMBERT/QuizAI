using FluentValidation;
using QuizAI.Application.Extensions;

namespace QuizAI.Application.Quizzes.Commands.UpdateQuizImage;

public class UpdateQuizImageCommandValidator :AbstractValidator<UpdateQuizImageCommand>
{
    public UpdateQuizImageCommandValidator()
    {
        RuleFor(qz => qz.Image)
            .IsValidImage();
    }
}

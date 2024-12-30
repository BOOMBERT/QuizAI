using FluentValidation;
using QuizAI.Application.Extensions;

namespace QuizAI.Application.Quizzes.Commands.CreateQuiz;

public class CreateQuizCommandValidator : AbstractValidator<CreateQuizCommand>
{
    public CreateQuizCommandValidator()
    {
        RuleFor(qz => qz.Name)
            .IsValidQuizName();

        RuleFor(qz => qz.Description)
            .IsValidQuizDescription();

        RuleFor(qz => qz.Image)
            .IsValidImage()
            .When(qz => qz.Image != null);

        RuleFor(qz => qz.Categories)
            .IsValidQuizCategories();
    }
}

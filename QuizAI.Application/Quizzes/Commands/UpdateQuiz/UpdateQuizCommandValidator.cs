using FluentValidation;
using QuizAI.Application.Extensions;

namespace QuizAI.Application.Quizzes.Commands.UpdateQuiz;

public class UpdateQuizCommandValidator : AbstractValidator<UpdateQuizCommand>
{
    public UpdateQuizCommandValidator()
    {
        RuleFor(qz => qz.Name)
            .IsValidQuizName();

        RuleFor(qz => qz.Description)
            .IsValidQuizDescription();

        RuleFor(qz => qz.Categories)
            .IsValidQuizCategories();
    }
}

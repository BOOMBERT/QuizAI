using FluentValidation;

namespace QuizAI.Application.Questions.Commands.AnswerCurrentQuestion;

public class AnswerCurrentQuestionCommandValidator : AbstractValidator<AnswerCurrentQuestionCommand>
{
    public AnswerCurrentQuestionCommandValidator()
    {
        RuleFor(qn => qn.UserAnswer)
            .Must(ua => ua.All(ua => !string.IsNullOrWhiteSpace(ua))).WithMessage("Answer cannot be empty or whitespace")
            .Must(ua => ua.ToHashSet().Count == ua.Count).WithMessage("Answers must be unique")
            .Must(ua => ua.Count <= 8).WithMessage("You can specify up to 8 answers")
            .Must(ua => ua.Sum(ua => ua.Length) <= 2040).WithMessage("The total length of all answers must not exceed 2040 characters");
    }
}

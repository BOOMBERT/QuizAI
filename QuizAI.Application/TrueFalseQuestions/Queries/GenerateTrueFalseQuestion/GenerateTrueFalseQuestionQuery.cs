using MediatR;
using QuizAI.Application.TrueFalseQuestions.Dtos;

namespace QuizAI.Application.TrueFalseQuestions.Queries.GenerateTrueFalseQuestion;

public class GenerateTrueFalseQuestionQuery(Guid quizId, string? suggestions) : IRequest<TrueFalseAnswerWithQuestionDto>
{
    public Guid QuizId { get; } = quizId;
    public string? Suggestions { get; } = suggestions;
}

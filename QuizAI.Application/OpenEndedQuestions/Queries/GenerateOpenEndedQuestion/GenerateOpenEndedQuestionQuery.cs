using MediatR;
using QuizAI.Application.OpenEndedQuestions.Dtos;

namespace QuizAI.Application.OpenEndedQuestions.Queries.GenerateOpenEndedQuestion;

public class GenerateOpenEndedQuestionQuery(Guid quizId, string? suggestions) : IRequest<OpenEndedAnswersWithQuestionDto>
{
    public Guid QuizId { get; } = quizId;
    public string? Suggestions { get; } = suggestions;
}

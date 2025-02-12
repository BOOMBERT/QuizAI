using MediatR;
using QuizAI.Application.QuizAttempts.Dtos;

namespace QuizAI.Application.QuizAttempts.Queries.GetAttemptById;

public class GetAttemptByIdQuery(Guid quizId, Guid quizAttemptId) : IRequest<QuizAttemptWithUserAnsweredQuestionsDto>
{
    public Guid QuizId { get; } = quizId;
    public Guid QuizAttemptId { get; } = quizAttemptId;
}

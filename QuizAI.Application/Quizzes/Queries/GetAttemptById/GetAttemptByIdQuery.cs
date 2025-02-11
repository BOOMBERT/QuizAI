using MediatR;
using QuizAI.Application.Quizzes.Dtos;

namespace QuizAI.Application.Quizzes.Queries.GetAttemptById;

public class GetAttemptByIdQuery(Guid quizId, Guid quizAttemptId) : IRequest<QuizAttemptWithUserAnsweredQuestionsDto>
{
    public Guid QuizId { get; } = quizId;
    public Guid QuizAttemptId { get; } = quizAttemptId;
}

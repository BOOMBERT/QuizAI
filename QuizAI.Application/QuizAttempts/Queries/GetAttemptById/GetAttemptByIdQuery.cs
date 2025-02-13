using MediatR;
using QuizAI.Application.QuizAttempts.Dtos;

namespace QuizAI.Application.QuizAttempts.Queries.GetAttemptById;

public class GetAttemptByIdQuery(Guid quizAttemptId) : IRequest<QuizAttemptViewWithUserAnsweredQuestionsDto>
{
    public Guid QuizAttemptId { get; } = quizAttemptId;
}

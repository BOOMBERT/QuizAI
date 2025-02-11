using MediatR;
using QuizAI.Application.Quizzes.Dtos;

namespace QuizAI.Application.Quizzes.Queries.GetLatestAttempt;

public class GetLatestAttemptQuery(Guid quizId) : IRequest<QuizAttemptWithUserAnsweredQuestionsDto>
{
    public Guid QuizId { get; } = quizId;
}

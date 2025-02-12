using MediatR;
using QuizAI.Application.QuizAttempts.Dtos;

namespace QuizAI.Application.QuizAttempts.Queries.GetLatestAttempt;

public class GetLatestAttemptQuery(Guid quizId) : IRequest<QuizAttemptWithUserAnsweredQuestionsDto>
{
    public Guid QuizId { get; } = quizId;
}

using MediatR;
using QuizAI.Application.QuizAttempts.Dtos;

namespace QuizAI.Application.QuizAttempts.Queries.GetLatestAttempt;

public class GetLatestAttemptQuery(Guid quizId) : IRequest<QuizAttemptViewWithUserAnsweredQuestionsDto>
{
    public Guid QuizId { get; } = quizId;
}

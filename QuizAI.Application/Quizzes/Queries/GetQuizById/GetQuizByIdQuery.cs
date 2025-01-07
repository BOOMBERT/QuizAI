using MediatR;
using QuizAI.Application.Quizzes.Dtos;

namespace QuizAI.Application.Quizzes.Queries.GetQuizById;

public class GetQuizByIdQuery(Guid quizId) : IRequest<QuizDto>
{
    public Guid QuizId { get; } = quizId;
}

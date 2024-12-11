using MediatR;

namespace QuizAI.Application.Quizzes.Queries.GetQuizImageById;

public class GetQuizImageByIdQuery(Guid quizId) : IRequest<(byte[], string)> 
{
    public Guid quizId { get; } = quizId;
}

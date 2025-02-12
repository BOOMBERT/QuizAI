using MediatR;

namespace QuizAI.Application.QuizImages.Queries.GetQuizImageById;

public class GetQuizImageByIdQuery(Guid quizId) : IRequest<(byte[], string)>
{
    public Guid QuizId { get; } = quizId;
}

using MediatR;

namespace QuizAI.Application.Questions.Queries.GetQuestionImageById;

public class GetQuestionImageByIdQuery(Guid quizId, int questionId) : IRequest<(byte[], string)>
{
    public Guid QuizId { get; } = quizId;
    public int QuestionId { get; } = questionId;
}

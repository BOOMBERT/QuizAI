using MediatR;
using QuizAI.Application.Questions.Dtos;

namespace QuizAI.Application.Questions.Queries.GetAllQuestions;

public class GetAllQuestionsQuery(Guid quizId) : IRequest<IEnumerable<QuestionWithAnswerDto>>
{
    public Guid QuizId { get; } = quizId;
}

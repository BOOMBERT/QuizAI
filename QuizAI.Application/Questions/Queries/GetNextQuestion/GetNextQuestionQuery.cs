using MediatR;
using QuizAI.Application.Questions.Dtos;
using QuizAI.Domain.Entities;

namespace QuizAI.Application.Questions.Queries.GetQuestion;

public class GetNextQuestionQuery(Guid quizId) : IRequest<QuestionDto>
{
    public Guid QuizId { get; } = quizId;
}

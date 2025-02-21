using MediatR;
using QuizAI.Application.MultipleChoiceQuestions.Dtos;

namespace QuizAI.Application.MultipleChoiceQuestions.Queries.GenerateMultipleChoiceQuestion;

public class GenerateMultipleChoiceQuestionQuery(Guid quizId, string? suggestions) : IRequest<MultipleChoiceAnswersWithQuestionDto>
{
    public Guid QuizId { get; } = quizId;
    public string? Suggestions { get; } = suggestions;
}
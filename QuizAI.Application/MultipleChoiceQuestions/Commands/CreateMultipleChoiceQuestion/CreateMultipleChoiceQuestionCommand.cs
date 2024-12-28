using MediatR;
using QuizAI.Application.MultipleChoiceQuestions.Dtos;

namespace QuizAI.Application.MultipleChoiceQuestions.Commands.CreateMultipleChoiceQuestion;

public class CreateMultipleChoiceQuestionCommand : IRequest<int>
{
    private Guid QuizId { get; set; }
    public required string Content { get; set; }
    public ICollection<CreateMultipleChoiceAnswerDto> Answers { get; set; } = new List<CreateMultipleChoiceAnswerDto>();

    public void SetQuizId(Guid id)
    {
        QuizId = id;
    }

    public Guid GetQuizId()
    {
        return QuizId;
    }
}

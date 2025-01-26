using MediatR;
using QuizAI.Application.Common;
using QuizAI.Application.MultipleChoiceQuestions.Dtos;

namespace QuizAI.Application.MultipleChoiceQuestions.Commands.UpdateMultipleChoiceQuestion;

public class UpdateMultipleChoiceQuestionCommand : IRequest<NewQuizId>
{
    private Guid QuizId { get; set; }
    private int QuestionId { get; set; }
    public string Content { get; set; } = string.Empty;
    public ICollection<CreateMultipleChoiceAnswerDto> Answers { get; set; } = new List<CreateMultipleChoiceAnswerDto>();

    public void SetQuizId(Guid id)
    {
        QuizId = id;
    }

    public Guid GetQuizId()
    {
        return QuizId;
    }

    public void SetQuestionId(int id)
    {
        QuestionId = id;
    }

    public int GetQuestionId()
    {
        return QuestionId;
    }
}

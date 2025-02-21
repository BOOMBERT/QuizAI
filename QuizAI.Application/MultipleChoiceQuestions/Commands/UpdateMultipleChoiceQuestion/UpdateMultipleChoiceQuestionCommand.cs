using MediatR;
using QuizAI.Application.Common;
using QuizAI.Application.MultipleChoiceQuestions.Dtos;

namespace QuizAI.Application.MultipleChoiceQuestions.Commands.UpdateMultipleChoiceQuestion;

public class UpdateMultipleChoiceQuestionCommand : IRequest<LatestQuizId>
{
    private Guid QuizId { get; set; }
    private int QuestionId { get; set; }
    public string Content { get; set; } = string.Empty;
    public ICollection<MultipleChoiceAnswerDto> Answers { get; set; } = new List<MultipleChoiceAnswerDto>();

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

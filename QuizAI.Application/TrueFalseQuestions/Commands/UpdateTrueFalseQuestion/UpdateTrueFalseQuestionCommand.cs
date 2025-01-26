using MediatR;
using QuizAI.Application.Common;
using QuizAI.Application.MultipleChoiceQuestions.Dtos;

namespace QuizAI.Application.TrueFalseQuestions.Commands.UpdateTrueFalseQuestion;

public class UpdateTrueFalseQuestionCommand : IRequest<NewQuizId>
{
    private Guid QuizId { get; set; }
    private int QuestionId { get; set; }
    public string Content { get; set; } = string.Empty;
    public bool IsCorrect { get; set; }

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

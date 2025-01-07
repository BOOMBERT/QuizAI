using MediatR;

namespace QuizAI.Application.Questions.Commands.DeleteQuestion;

public class DeleteQuestionCommand : IRequest
{
    private Guid QuizId { get; set; }
    private int QuestionId { get; set; }

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

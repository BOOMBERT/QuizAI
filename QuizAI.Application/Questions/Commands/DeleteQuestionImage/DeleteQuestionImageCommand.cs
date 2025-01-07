using MediatR;
using Microsoft.AspNetCore.Http;

namespace QuizAI.Application.Questions.Commands.DeleteQuestionImage;

public class DeleteQuestionImageCommand : IRequest
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

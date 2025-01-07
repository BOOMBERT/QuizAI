using MediatR;
using Microsoft.AspNetCore.Http;

namespace QuizAI.Application.Questions.Commands.UpdateQuestionImage;

public class UpdateQuestionImageCommand : IRequest
{
    private Guid QuizId { get; set; }
    private int QuestionId { get; set; }
    public required IFormFile Image { get; set; }

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

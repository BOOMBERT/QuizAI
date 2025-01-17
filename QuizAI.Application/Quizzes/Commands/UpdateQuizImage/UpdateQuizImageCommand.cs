using MediatR;
using Microsoft.AspNetCore.Http;
using QuizAI.Application.Common;

namespace QuizAI.Application.Quizzes.Commands.UpdateQuizImage;

public class UpdateQuizImageCommand : IRequest<NewQuizId>
{
    private Guid Id { get; set; }
    public required IFormFile Image { get; set; }

    public void SetId(Guid id)
    {
        Id = id;
    }

    public Guid GetId()
    {
        return Id;
    }
}

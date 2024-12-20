using MediatR;
using Microsoft.AspNetCore.Http;

namespace QuizAI.Application.Quizzes.Commands.UpdateQuizImage;

public class UpdateQuizImageCommand : IRequest
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

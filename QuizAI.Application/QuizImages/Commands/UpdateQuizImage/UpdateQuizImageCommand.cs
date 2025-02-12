using MediatR;
using Microsoft.AspNetCore.Http;
using QuizAI.Application.Common;

namespace QuizAI.Application.QuizImages.Commands.UpdateQuizImage;

public class UpdateQuizImageCommand : IRequest<LatestQuizId>
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

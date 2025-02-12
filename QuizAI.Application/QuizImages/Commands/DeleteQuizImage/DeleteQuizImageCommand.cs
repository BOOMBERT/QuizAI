using MediatR;
using QuizAI.Application.Common;

namespace QuizAI.Application.QuizImages.Commands.DeleteQuizImage;

public class DeleteQuizImageCommand : IRequest<LatestQuizId>
{
    private Guid Id { get; set; }

    public void SetId(Guid id)
    {
        Id = id;
    }

    public Guid GetId()
    {
        return Id;
    }
}

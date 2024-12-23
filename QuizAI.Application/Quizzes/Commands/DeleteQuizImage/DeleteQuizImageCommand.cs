using MediatR;

namespace QuizAI.Application.Quizzes.Commands.DeleteQuizImage;

public class DeleteQuizImageCommand : IRequest
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

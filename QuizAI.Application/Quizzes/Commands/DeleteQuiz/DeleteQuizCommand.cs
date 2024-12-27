using MediatR;

namespace QuizAI.Application.Quizzes.Commands.DeleteQuiz;

public class DeleteQuizCommand : IRequest
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

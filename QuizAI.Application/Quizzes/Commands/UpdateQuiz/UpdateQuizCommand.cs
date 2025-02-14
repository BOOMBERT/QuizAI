using MediatR;
using QuizAI.Application.Common;

namespace QuizAI.Application.Quizzes.Commands.UpdateQuiz;

public class UpdateQuizCommand : IRequest<LatestQuizId>
{
    private Guid Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public ICollection<string> Categories { get; set; } = new List<string>();
    public bool IsPrivate { get; set; }

    public void SetId(Guid id)
    {
        Id = id;
    }

    public Guid GetId()
    {
        return Id;
    }
}

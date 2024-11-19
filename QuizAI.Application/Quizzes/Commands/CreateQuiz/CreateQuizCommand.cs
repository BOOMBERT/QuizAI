using MediatR;

namespace QuizAI.Application.Quizzes.Commands.CreateQuiz;

public class CreateQuizCommand : IRequest
{
    public required string Name { get; set; }
    public string? Description { get; set; }

    public ICollection<string> Categories { get; set; } = new List<string>();
}

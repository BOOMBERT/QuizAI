using MediatR;
using Microsoft.AspNetCore.Http;

namespace QuizAI.Application.Quizzes.Commands.CreateQuiz;

public class CreateQuizCommand : IRequest<Guid>
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public IFormFile? Image { get; set; }
    public bool IsPrivate { get; set; }
    public ICollection<string> Categories { get; set; } = new List<string>();
}

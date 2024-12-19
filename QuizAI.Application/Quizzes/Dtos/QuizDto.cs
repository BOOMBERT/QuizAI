namespace QuizAI.Application.Quizzes.Dtos;

public record QuizDto(string Name, string? Description, DateTime CreationDate, bool HasImage, IEnumerable<string> Categories);

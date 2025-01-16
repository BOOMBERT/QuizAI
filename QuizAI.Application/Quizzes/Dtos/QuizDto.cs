namespace QuizAI.Application.Quizzes.Dtos;

public record QuizDto(
    Guid Id, 
    string Name, 
    string? Description, 
    DateTime CreationDate, 
    bool HasImage, 
    IEnumerable<string> Categories, 
    bool IsDeprecated, 
    Guid? LatestVersionId
    );

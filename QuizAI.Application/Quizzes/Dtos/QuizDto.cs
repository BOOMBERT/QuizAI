namespace QuizAI.Application.Quizzes.Dtos;

public record QuizDto(
    Guid Id, 
    string Name, 
    string? Description, 
    DateTime CreationDate, 
    bool HasImage, 
    IEnumerable<string> Categories,
    bool IsPrivate,
    bool IsDeprecated,
    Guid? LatestVersionId,
    int QuestionCount,
    string CreatorId,
    bool CanEdit
    );

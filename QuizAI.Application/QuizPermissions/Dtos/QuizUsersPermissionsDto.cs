namespace QuizAI.Application.QuizPermissions.Dtos;

public record QuizUsersPermissionsDto(Guid Id, string UserEmail, bool CanEdit, bool CanPlay);

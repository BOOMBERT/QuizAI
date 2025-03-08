using MediatR;

namespace QuizAI.Application.QuizPermissions.Commands.ManageUsersQuizPermissions;

public class ManageUsersQuizPermissionsCommand : IRequest
{
    private Guid QuizId { get; set; }
    private string UserEmail { get; set; } = string.Empty;
    public bool CanEdit { get; set; }
    public bool CanPlay { get; set; }

    public void SetQuizId(Guid id)
    {
        QuizId = id;
    }

    public Guid GetQuizId()
    {
        return QuizId;
    }

    public void SetUserEmail(string email)
    {
        UserEmail = email;
    }

    public string GetUserEmail()
    {
        return UserEmail;
    }
}

using MediatR;

namespace QuizAI.Application.Users.Commands.ChangeUserPassword;

public class ChangeUserPasswordCommand(string oldPassword, string newPassword) : IRequest
{
    public string OldPassword { get; } = oldPassword;
    public string NewPassword { get; } = newPassword;
}

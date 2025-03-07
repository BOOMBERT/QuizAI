using MediatR;

namespace QuizAI.Application.Users.Commands.ResendUserConfirmationEmail;

public class ResendUserConfirmationEmailCommand : IRequest
{
    public required string Email { get; set; }
}

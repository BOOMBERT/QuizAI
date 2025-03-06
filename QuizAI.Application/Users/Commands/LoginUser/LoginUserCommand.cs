using MediatR;

namespace QuizAI.Application.Users.Commands.LoginUser;

public class LoginUserCommand : IRequest
{
    public required string Email { get; init; }
    public required string Password { get; init; }
}

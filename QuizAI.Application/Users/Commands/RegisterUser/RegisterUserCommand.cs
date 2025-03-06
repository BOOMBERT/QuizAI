using MediatR;

namespace QuizAI.Application.Users.Commands.RegisterUser;

public class RegisterUserCommand : IRequest
{
    public required string Email { get; init; }
    public required string Password { get; init; }
}

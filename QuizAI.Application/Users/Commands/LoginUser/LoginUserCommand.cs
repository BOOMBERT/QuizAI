using MediatR;
using System.ComponentModel.DataAnnotations;

namespace QuizAI.Application.Users.Commands.LoginUser;

public class LoginUserCommand : IRequest
{
    [EmailAddress]
    public required string Email { get; init; }
    public required string Password { get; init; }
}

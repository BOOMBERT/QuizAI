using MediatR;

namespace QuizAI.Application.Users.Commands.ConfirmUserEmail;

public class ConfirmUserEmailCommand : IRequest
{
    public required string Id { get; set; }
    public required string Token { get; set; }
}

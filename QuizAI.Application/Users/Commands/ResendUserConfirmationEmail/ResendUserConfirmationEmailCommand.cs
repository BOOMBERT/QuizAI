using MediatR;
using System.ComponentModel.DataAnnotations;

namespace QuizAI.Application.Users.Commands.ResendUserConfirmationEmail;

public class ResendUserConfirmationEmailCommand : IRequest
{
    [EmailAddress]
    public required string Email { get; set; }
}

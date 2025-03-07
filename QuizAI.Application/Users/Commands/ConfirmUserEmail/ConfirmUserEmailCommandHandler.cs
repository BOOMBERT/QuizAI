using MediatR;
using Microsoft.AspNetCore.Identity;
using QuizAI.Domain.Entities;
using QuizAI.Domain.Exceptions;

namespace QuizAI.Application.Users.Commands.ConfirmUserEmail;

public class ConfirmUserEmailCommandHandler : IRequestHandler<ConfirmUserEmailCommand>
{
    private readonly UserManager<User> _userManager;

    public ConfirmUserEmailCommandHandler(UserManager<User> userManager)
    {
        _userManager = userManager;
    }

    public async Task Handle(ConfirmUserEmailCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.Id)
            ?? throw new BadRequestException("Invalid email confirmation request");

        if (await _userManager.IsEmailConfirmedAsync(user))
            throw new BadRequestException($"Email {user.Email} is already confirmed");

        var decodedToken = Uri.UnescapeDataString(request.Token);

        var confirmResult = await _userManager.ConfirmEmailAsync(user, decodedToken);
        if (!confirmResult.Succeeded)
            throw new BadRequestException("Invalid email confirmation request");
    }
}

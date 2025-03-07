using MediatR;
using Microsoft.AspNetCore.Identity;
using QuizAI.Application.Interfaces;
using QuizAI.Domain.Entities;
using QuizAI.Domain.Exceptions;

namespace QuizAI.Application.Users.Commands.ResendUserConfirmationEmail;

public class ResendUserConfirmationEmailCommandHandler : IRequestHandler<ResendUserConfirmationEmailCommand>
{
    private readonly UserManager<User> _userManager;
    private readonly IRabbitMqService _rabbitMqService;

    public ResendUserConfirmationEmailCommandHandler(UserManager<User> userManager, IRabbitMqService rabbitMqService)
    {
        _userManager = userManager;
        _rabbitMqService = rabbitMqService;
    }

    public async Task Handle(ResendUserConfirmationEmailCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.Email)
            ?? throw new BadRequestException("Invalid resend email confirmation request");

        if (await _userManager.IsEmailConfirmedAsync(user))
            throw new BadRequestException($"Email {user.Email} is already confirmed");

        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        var encodedToken = Uri.EscapeDataString(token);

        await _rabbitMqService.SendRegistrationConfirmationEmailToQueueAsync(user, encodedToken);
    }
}

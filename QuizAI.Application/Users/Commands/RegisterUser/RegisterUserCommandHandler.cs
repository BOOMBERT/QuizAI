using MediatR;
using Microsoft.AspNetCore.Identity;
using QuizAI.Domain.Entities;
using QuizAI.Domain.Exceptions;
using QuizAI.Domain.Interfaces;
using QuizAI.Domain.Repositories;

namespace QuizAI.Application.Users.Commands.RegisterUser;

public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand>
{
    private readonly UserManager<User> _userManager;
    private readonly IUserRepository _userRepository;
    private readonly IRabbitMqService _rabbitMqService;

    public RegisterUserCommandHandler(UserManager<User> userManager, IUserRepository userRepository, IRabbitMqService rabbitMqService)
    {
        _userManager = userManager;
        _userRepository = userRepository;
        _rabbitMqService = rabbitMqService;
    }

    public async Task Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        if (await _userRepository.ExistsByEmailAsync(request.Email))
            throw new ConflictException($"User with email ({request.Email}) already exists");

        var user = User.Create(request.Email);

        var result = await _userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
            throw new UnprocessableEntityException(string.Join(", ", result.Errors.Select(e => e.Description.TrimEnd('.'))));

        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        var encodedToken = Uri.EscapeDataString(token);

        await _rabbitMqService.SendRegistrationConfirmationEmailToQueueAsync(user, encodedToken);
    }
}

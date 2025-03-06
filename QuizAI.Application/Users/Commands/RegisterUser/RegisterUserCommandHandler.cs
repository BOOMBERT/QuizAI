using MediatR;
using Microsoft.AspNetCore.Identity;
using QuizAI.Domain.Entities;
using QuizAI.Domain.Exceptions;
using QuizAI.Domain.Repositories;

namespace QuizAI.Application.Users.Commands.RegisterUser;

public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand>
{
    private readonly UserManager<User> _userManager;
    private readonly IUserRepository _userRepository;

    public RegisterUserCommandHandler(UserManager<User> userManager, IUserRepository userRepository)
    {
        _userManager = userManager;
        _userRepository = userRepository;
    }

    public async Task Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        if (await _userRepository.ExistsByEmailAsync(request.Email))
            throw new ConflictException($"User with email ({request.Email}) already exists");

        var result = await _userManager.CreateAsync(User.Create(request.Email), request.Password);

        if (!result.Succeeded)
            throw new UnprocessableEntityException(string.Join(", ", result.Errors.Select(e => e.Description.TrimEnd('.'))));
    }
}

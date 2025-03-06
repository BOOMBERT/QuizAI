using MediatR;
using Microsoft.AspNetCore.Identity;
using QuizAI.Application.Interfaces;
using QuizAI.Domain.Entities;
using QuizAI.Domain.Exceptions;

namespace QuizAI.Application.Users.Commands.ChangeUserPassword;

public class ChangeUserPasswordCommandHandler : IRequestHandler<ChangeUserPasswordCommand>
{
    private readonly IUserContext _userContext;
    private readonly UserManager<User> _userManager;

    public ChangeUserPasswordCommandHandler(UserManager<User> userManager, IUserContext userContext)
    {
        _userContext = userContext;
        _userManager = userManager;
    }

    public async Task Handle(ChangeUserPasswordCommand request, CancellationToken cancellationToken)
    {
        var currentUser = _userContext.GetCurrentUser();

        if (request.OldPassword == request.NewPassword)
            throw new BadRequestException("New password cannot be the same as the old password");

        var user = await _userManager.FindByIdAsync(currentUser.Id) 
            ?? throw new NotFoundException("User not found");
        
        var result = await _userManager.ChangePasswordAsync(user, request.OldPassword, request.NewPassword);

        if (!result.Succeeded)
            throw new UnprocessableEntityException(string.Join(", ", result.Errors.Select(e => e.Description.TrimEnd('.'))));
    }
}

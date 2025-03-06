using MediatR;
using Microsoft.AspNetCore.Identity;
using QuizAI.Domain.Entities;
using QuizAI.Application.Interfaces;
using QuizAI.Domain.Exceptions;

namespace QuizAI.Application.Users.Commands.LoginUser;

public class LoginUserCommandHandler : IRequestHandler<LoginUserCommand>
{
    private readonly UserManager<User> _userManager;
    private readonly IAuthenticationService _authenticationService;

    public LoginUserCommandHandler(UserManager<User> userManager, IAuthenticationService authenticationService)
    {
        _userManager = userManager;
        _authenticationService = authenticationService;
    }

    public async Task Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);

        if (user == null || !await _userManager.CheckPasswordAsync(user, request.Password))
            throw new UnauthorizedException($"Invalid email ({request.Email}) or password");

        var (jwtToken, jwtTokenExpirationDateInUtc) = _authenticationService.GenerateJwtToken(user);
        var (refreshToken, refreshTokenExpirationDateInUtc) = _authenticationService.GenerateRefreshToken();

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiresAtUtc = refreshTokenExpirationDateInUtc;

        await _userManager.UpdateAsync(user);

        _authenticationService.WriteAuthTokenAsHttpOnlyCookie("ACCESS_TOKEN", jwtToken, jwtTokenExpirationDateInUtc);
        _authenticationService.WriteAuthTokenAsHttpOnlyCookie("REFRESH_TOKEN", user.RefreshToken, refreshTokenExpirationDateInUtc);
    }
}

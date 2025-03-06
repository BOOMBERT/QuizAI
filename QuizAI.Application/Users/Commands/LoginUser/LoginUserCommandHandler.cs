using MediatR;
using Microsoft.AspNetCore.Identity;
using QuizAI.Domain.Entities;
using QuizAI.Application.Interfaces;
using QuizAI.Domain.Exceptions;

namespace QuizAI.Application.Users.Commands.LoginUser;

public class LoginUserCommandHandler : IRequestHandler<LoginUserCommand>
{
    private readonly UserManager<User> _userManager;
    private readonly IAuthService _authService;

    public LoginUserCommandHandler(UserManager<User> userManager, IAuthService authService)
    {
        _userManager = userManager;
        _authService = authService;
    }

    public async Task Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);

        if (user == null || !await _userManager.CheckPasswordAsync(user, request.Password))
            throw new UnauthorizedException($"Invalid email ({request.Email}) or password");

        var (jwtToken, jwtTokenExpirationDateInUtc) = _authService.GenerateJwtToken(user);
        var (refreshToken, refreshTokenExpirationDateInUtc) = _authService.GenerateRefreshToken();

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiresAtUtc = refreshTokenExpirationDateInUtc;

        await _userManager.UpdateAsync(user);

        _authService.WriteAuthTokenAsHttpOnlyCookie("ACCESS_TOKEN", jwtToken, jwtTokenExpirationDateInUtc);
        _authService.WriteAuthTokenAsHttpOnlyCookie("REFRESH_TOKEN", user.RefreshToken, refreshTokenExpirationDateInUtc);
    }
}

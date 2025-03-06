using MediatR;
using Microsoft.AspNetCore.Identity;
using QuizAI.Application.Interfaces;
using QuizAI.Domain.Entities;
using QuizAI.Domain.Exceptions;
using QuizAI.Domain.Repositories;

namespace QuizAI.Application.Users.Commands.RefreshUserTokens;

public class RefreshUserTokensCommandHandler : IRequestHandler<RefreshUserTokensCommand>
{
    private readonly IUserRepository _userRepository;
    private readonly IAuthService _authService;
    private readonly UserManager<User> _userManager;

    public RefreshUserTokensCommandHandler(IUserRepository userRepository, IAuthService authService, UserManager<User> userManager)
    {
        _userRepository = userRepository;
        _authService = authService;
        _userManager = userManager;
    }

    public async Task Handle(RefreshUserTokensCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(request.RefreshToken))        
            throw new BadRequestException("Refresh token is missing");

        var user = await _userRepository.GetUserByRefreshTokenAsync(request.RefreshToken) 
            ?? throw new UnauthorizedException("Invalid refresh token");

        if (user.RefreshTokenExpiresAtUtc < DateTime.UtcNow)
            throw new ForbiddenException("Refresh token is expired");

        var (jwtToken, jwtTokenExpirationDateInUtc) = _authService.GenerateJwtToken(user);
        var (refreshToken, refreshTokenExpirationDateInUtc) = _authService.GenerateRefreshToken();

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiresAtUtc = refreshTokenExpirationDateInUtc;

        await _userManager.UpdateAsync(user);

        _authService.WriteAuthTokenAsHttpOnlyCookie("ACCESS_TOKEN", jwtToken, jwtTokenExpirationDateInUtc);
        _authService.WriteAuthTokenAsHttpOnlyCookie("REFRESH_TOKEN", user.RefreshToken, refreshTokenExpirationDateInUtc);
    }
}

using MediatR;

namespace QuizAI.Application.Users.Commands.RefreshUserTokens;

public class RefreshUserTokensCommand(string? refreshToken) : IRequest
{
    public string? RefreshToken { get; } = refreshToken;
}

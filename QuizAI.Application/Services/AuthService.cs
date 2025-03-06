using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using QuizAI.Application.Interfaces;
using QuizAI.Domain.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace QuizAI.Application.Services;

public class AuthService : IAuthService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly string _jwtKey;
    private readonly double _accessTokenExpirationInMinutes;
    private readonly double _refreshTokenExpirationInMinutes;

    public AuthService(IHttpContextAccessor httpContextAccessor, string jwtKey, double accessTokenExpirationInMinutes, double refreshTokenExpirationInMinutes)
    {
        _httpContextAccessor = httpContextAccessor;
        _jwtKey = jwtKey;
        _accessTokenExpirationInMinutes = accessTokenExpirationInMinutes;
        _refreshTokenExpirationInMinutes = refreshTokenExpirationInMinutes;
    }
    
    public (string jwtToken, DateTime expiresAtUtc) GenerateJwtToken(User user)
    {
        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtKey));

        var credentials = new SigningCredentials(
            signingKey,
            SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Email, user.Email!),
        };

        var expires = DateTime.UtcNow.AddMinutes(_accessTokenExpirationInMinutes);

        var token = new JwtSecurityToken(
            claims: claims,
            expires: expires,
            signingCredentials: credentials);

        var jwtToken = new JwtSecurityTokenHandler().WriteToken(token);

        return (jwtToken, expires);
    }

    public (string refreshToken, DateTime expiresAtUtc) GenerateRefreshToken()
    {
        var randomNumber = new byte[64];
        using var randomNumberGenerator = RandomNumberGenerator.Create();
        randomNumberGenerator.GetBytes(randomNumber);

        var expires = DateTime.UtcNow.AddMinutes(_refreshTokenExpirationInMinutes);

        return (Convert.ToBase64String(randomNumber), expires);
    }

    public void WriteAuthTokenAsHttpOnlyCookie(string cookieName, string token, DateTime expiration)
    {
        _httpContextAccessor.HttpContext!.Response.Cookies.Append(cookieName, token, 
            new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                Expires = expiration,
                SameSite = SameSiteMode.Strict
            });
    }
}

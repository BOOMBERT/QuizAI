﻿using QuizAI.Domain.Entities;

namespace QuizAI.Application.Interfaces;

public interface IAuthService
{
    (string jwtToken, DateTime expiresAtUtc) GenerateJwtToken(User user);
    (string refreshToken, DateTime expiresAtUtc) GenerateRefreshToken();
    void WriteAuthTokenAsHttpOnlyCookie(string cookieName, string token, DateTime expiration);
}

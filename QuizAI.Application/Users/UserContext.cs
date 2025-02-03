﻿using Microsoft.AspNetCore.Http;
using QuizAI.Application.Interfaces;
using QuizAI.Domain.Exceptions;
using System.Security.Claims;

namespace QuizAI.Application.Users;

public class UserContext : IUserContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserContext(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public CurrentUser? GetCurrentUser()
    {
        var user = _httpContextAccessor?.HttpContext?.User 
            ?? throw new NotFoundException("The user could not be retrieved from HttpContext.");

        if (user.Identity == null || !user.Identity.IsAuthenticated)
            return null;

        var userId = user.FindFirst(c => c.Type == ClaimTypes.NameIdentifier)!.Value;
        var email = user.FindFirst(c => c.Type == ClaimTypes.Email)!.Value;

        return new CurrentUser(userId, email);
    }
}

using QuizAI.Application.Interfaces;
using QuizAI.Application.Users;
using QuizAI.Domain.Constants;
using QuizAI.Domain.Entities;
using QuizAI.Domain.Exceptions;
using QuizAI.Domain.Repositories;

namespace QuizAI.Application.Services;

public class QuizAuthorizationService : IQuizAuthorizationService
{
    private readonly IUserContext _userContext;
    private readonly IQuizPermissionsRepository _quizPermissionsRepository;

    public QuizAuthorizationService(IUserContext userContext, IQuizPermissionsRepository quizPermissionsRepository)
    {
        _userContext = userContext;
        _quizPermissionsRepository = quizPermissionsRepository;
    }

    public async Task<bool> AuthorizeAsync(Quiz quiz, string? userId, ResourceOperation resourceOperation)
    {
        if (userId == null)
            userId = _userContext.GetCurrentUser().Id;

        return resourceOperation switch
        {
            ResourceOperation.Read => await AuthorizeReadOperationAsync(quiz, userId),
            ResourceOperation.RestrictedRead => await AuthorizeRestrictedReadOperationAsync(quiz, userId),
            _ => false
        };
    }

    private async Task<bool> AuthorizeReadOperationAsync(Quiz quiz, string userId)
    {
        if (!quiz.IsPrivate) 
            return true;

        var isAuthorized = (
            quiz.CreatorId == userId || 
            await _quizPermissionsRepository.HasAnyAsync(quiz.Id, userId)
            );

        if (!isAuthorized) 
            throw new ForbiddenException("You do not have permission to access this resource");

        return true;
    }

    private async Task<bool> AuthorizeRestrictedReadOperationAsync(Quiz quiz, string userId)
    {
        var isAuthorized = (
            quiz.CreatorId == userId ||
            await _quizPermissionsRepository.CanEditAsync(quiz.Id, userId)
            );

        if (!isAuthorized)
            throw new ForbiddenException("You do not have permission to access this resource");

        return true;
    }

}

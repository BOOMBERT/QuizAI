using QuizAI.Application.Interfaces;
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

    public async Task AuthorizeAsync(Quiz quiz, string? userId, ResourceOperation resourceOperation)
    {
        if (userId == null)
            userId = _userContext.GetCurrentUser().Id;

        switch (resourceOperation)
        {
            case ResourceOperation.Create:
                await AuthorizeBasicAccessAsync(quiz, userId);
                break;
            case ResourceOperation.Read:
                await AuthorizeBasicAccessAsync(quiz, userId);
                break;
            case ResourceOperation.RestrictedRead:
                await AuthorizeRestrictedAccessAsync(quiz, userId);
                break;
            case ResourceOperation.Update:
                await AuthorizeRestrictedAccessAsync(quiz, userId);
                break;
            case ResourceOperation.Delete:
                AuthorizeDeleteOperation(quiz, userId);
                break;
            default:
                throw new ForbiddenException($"Authorization for {resourceOperation} is not implemented");
        }
    }

    public async Task<bool> AuthorizeReadOperationAndGetCanEditAsync(Quiz quiz, string? userId = null)
    {
        if (userId == null)
            userId = _userContext.GetCurrentUser().Id;

        if (quiz.CreatorId == userId)
            return true;

        var userQuizPermissions = await _quizPermissionsRepository.GetAsync(quiz.Id, userId, false);

        if (!quiz.IsPrivate)
            return userQuizPermissions?.CanEdit ?? false;

        if (userQuizPermissions == null || (!userQuizPermissions?.CanPlay ?? false))
            throw new ForbiddenException("You do not have permission to access this resource");

        return userQuizPermissions?.CanEdit ?? false;
    }

    private async Task AuthorizeBasicAccessAsync(Quiz quiz, string userId)
    {
        var isAuthorized = (
            !quiz.IsPrivate ||
            quiz.CreatorId == userId ||
            await _quizPermissionsRepository.CheckAsync(quiz.Id, userId, null, true)
            );

        if (!isAuthorized)
            throw new ForbiddenException("You do not have permission to access this resource");
    }

    private async Task AuthorizeRestrictedAccessAsync(Quiz quiz, string userId)
    {
        var isAuthorized = (
            quiz.CreatorId == userId ||
            await _quizPermissionsRepository.CheckAsync(quiz.Id, userId, true, null)
            );

        if (!isAuthorized)
            throw new ForbiddenException("You do not have permission to access this resource");
    }

    private static void AuthorizeDeleteOperation(Quiz quiz, string userId)
    {
        if (quiz.CreatorId != userId)
            throw new ForbiddenException("You do not have permission to access this resource");
    }

}

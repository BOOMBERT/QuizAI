using MediatR;
using QuizAI.Application.Interfaces;
using QuizAI.Application.Quizzes.Dtos;
using QuizAI.Domain.Exceptions;
using QuizAI.Domain.Repositories;

namespace QuizAI.Application.Quizzes.Queries.GetQuizStats;

public class GetQuizStatsQueryHandler : IRequestHandler<GetQuizStatsQuery, QuizStatsDto>
{
    private readonly IUserContext _userContext;
    private readonly IQuizzesRepository _quizzesRepository;
    private readonly IQuizAttemptsRepository _quizAttemptsRepository;

    public GetQuizStatsQueryHandler(IUserContext userContext, IQuizzesRepository quizzesRepository, IQuizAttemptsRepository quizAttemptsRepository)
    {
        _userContext = userContext;
        _quizzesRepository = quizzesRepository;
        _quizAttemptsRepository = quizAttemptsRepository;
    }

    public async Task<QuizStatsDto> Handle(GetQuizStatsQuery request, CancellationToken cancellationToken)
    {
        var currentUser = _userContext.GetCurrentUser();

        var (creatorId, isDeprecated, latestVersionId) = await _quizzesRepository.GetCreatorIdAndIsDeprecatedAndLatestVersionIdAsync(request.QuizId)
            ?? throw new NotFoundException($"Quiz with ID {request.QuizId} was not found");

        if (creatorId != currentUser.Id) 
            throw new ForbiddenException($"You do not have permission to view the stats of the quiz with ID {request.QuizId} because you are not its creator");

        if (isDeprecated)
            throw new NotFoundQuizWithVersioningException(request.QuizId, latestVersionId);

        var (quizAttemptsCount, averageCorrectAnswers, averageTimeSpent) = 
            await _quizAttemptsRepository.GetDetailedStatsAsync(request.QuizId, request.IncludeDeprecatedVersions);

        return new QuizStatsDto(quizAttemptsCount, averageCorrectAnswers, averageTimeSpent);
    }
}

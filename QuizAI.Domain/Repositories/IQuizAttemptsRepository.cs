using QuizAI.Domain.Constants;
using QuizAI.Domain.Entities;

namespace QuizAI.Domain.Repositories;

public interface IQuizAttemptsRepository
{
    Task<QuizAttempt?> GetUnfinishedAsync(Guid quizId, string userId);
    Task<QuizAttempt?> GetFinishedByIdAsync(Guid quizAttemptId, string userId);
    Task<QuizAttempt?> GetLatestFinishedAsync(Guid quizId, string userId);
    Task<(IEnumerable<QuizAttempt>, int)> GetAllMatchingFinishedAsync(
        string userId,
        string? searchPhrase,
        int pageSize,
        int pageNumber,
        string? sortBy,
        SortDirection? sortDirection,
        Guid? filterQuizId,
        DateTime? filterStartedAt,
        DateTime? filterFinishedAt);
    Task<(int, double, TimeSpan)> GetDetailedStatsAsync(Guid quizId, bool includeDeprecated);
    Task<bool> HasAnyAsync(Guid quizId);
}

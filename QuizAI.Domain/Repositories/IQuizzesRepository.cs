using QuizAI.Domain.Constants;
using QuizAI.Domain.Entities;

namespace QuizAI.Domain.Repositories;

public interface IQuizzesRepository
{
    Task<Quiz?> GetAsync(Guid quizId, bool includeCategories = false, bool includeQuestionsWithAnswers = false, bool trackChanges = true);
    Task<(IEnumerable<Quiz>, int)> GetAllMatchingAsync(
        string? searchPhrase,
        int pageSize,
        int pageNumber,
        string? sortBy,
        SortDirection? sortDirection,
        string? filterUserId,
        ICollection<string> FilterCategories,
        string? UserIdToFilterBySharedQuizzes
        );
    Task<int?> GetQuestionCountAsync(Guid quizId);
    Task<(string, int)?> GetNameAndQuestionCountAsync(Guid quizId);
    Task<(string, bool, bool)?> GetCreatorIdAndIsPrivateAndIsDeprecatedAsync(Guid quizId);
    Task<(string, bool)?> GetCreatorIdAndIsDeprecatedAsync(Guid quizId);
    Task UpdateLatestVersionIdAsync(Guid oldLatestVersionId, Guid? newLatestVersionId);
}

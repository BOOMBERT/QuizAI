using QuizAI.Domain.Constants;
using QuizAI.Domain.Entities;

namespace QuizAI.Domain.Repositories;

public interface IQuizzesRepository
{
    Task<Quiz?> GetAsync(Guid quizId, bool includeCategories = false, bool includeQuestionsWithAnswers = false, bool trackChanges = true);
    Task<Quiz?> GetWithQuestionsAndCategoriesAsync(Guid quizId);
    Task<(IEnumerable<Quiz>, int)> GetAllMatchingAsync(
        string userId,
        string? searchPhrase,
        int pageSize,
        int pageNumber,
        string? sortBy,
        SortDirection? sortDirection,
        bool filterByCreatorId,
        ICollection<string> filterCategories,
        bool filterBySharedQuizzes,
        bool filterByUnfinishedAttempts
        );
    Task<(string, int, bool)?> GetNameAndQuestionCountAndIsPrivateAsync(Guid quizId);
    Task<(string, bool, bool, Guid?)?> GetCreatorIdAndIsPrivateAndIsDeprecatedAndLatestVersionIdAsync(Guid quizId);
    Task<(string, bool, Guid?)?> GetCreatorIdAndIsDeprecatedAndLatestVersionIdAsync(Guid quizId);
    Task UpdateLatestVersionIdAsync(Guid oldLatestVersionId, Guid? newLatestVersionId);
}

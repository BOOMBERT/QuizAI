using QuizAI.Domain.Constants;
using QuizAI.Domain.Entities;

namespace QuizAI.Domain.Repositories;

public interface IQuizzesRepository
{
    Task<Quiz?> GetWithCategoriesAsync(Guid quizId);
    Task<Guid?> GetImageIdAsync(Guid quizId);
    Task<(IEnumerable<Quiz>, int)> GetAllMatchingAsync(
        string? searchPhrase,
        int pageSize,
        int pageNumber,
        string? sortBy,
        SortDirection? sortDirection,
        ICollection<string> FilterCategories);
    Task<ICollection<Question>> GetQuestions(Guid quizId);
    Task UpdateImageAsync(Guid quizId, Guid? imageId);
    Task<IEnumerable<Guid>> GetQuestionsImagesAsync(Guid quizId);
    Task<int> HowManyQuestions(Guid quizId);
}

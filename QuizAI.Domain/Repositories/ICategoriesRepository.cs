using QuizAI.Domain.Entities;

namespace QuizAI.Domain.Repositories;

public interface ICategoriesRepository
{
    Task<IEnumerable<Category>> GetExistingAsync(IEnumerable<string> categoriesNames);
    Task<bool> IsAssignedToSingleQuizAsync(int categoryId);
}

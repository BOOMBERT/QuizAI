using QuizAI.Domain.Entities;

namespace QuizAI.Domain.Repositories;

public interface ICategoriesRepository
{
    Task<IEnumerable<Category>> GetExistingCategoriesAsync(IEnumerable<string> categoriesNames);
    Task<bool> IsAssignedToSingleQuiz(int categoryId);
}

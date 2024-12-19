using QuizAI.Domain.Entities;

namespace QuizAI.Application.Interfaces;

public interface ICategoryService
{
    Task<ICollection<Category>> GetOrCreateEntitiesAsync(IEnumerable<string> categoryNames);
    Task RemoveUnusedAsync(Quiz quiz, IEnumerable<string> requestCategories);
}

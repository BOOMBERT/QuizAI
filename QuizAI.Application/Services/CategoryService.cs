using QuizAI.Application.Interfaces;
using QuizAI.Domain.Entities;
using QuizAI.Domain.Repositories;

namespace QuizAI.Application.Services;

public class CategoryService : ICategoryService
{
    private readonly IRepository _repository;
    private readonly ICategoriesRepository _categoriesRepository;

    public CategoryService(IRepository repository, ICategoriesRepository categoriesRepository)
    {
        _repository = repository;
        _categoriesRepository = categoriesRepository;
    }

    public async Task<ICollection<Category>> GetOrCreateEntitiesAsync(IEnumerable<string> categoryNames)
    {
        if (!categoryNames.Any())
            return new List<Category>();

        var existingCategories = await _categoriesRepository.GetExistingCategoriesAsync(categoryNames);
        var existingCategoriesNames = existingCategories.Select(ec => ec.Name).ToHashSet();

        var newCategories = categoryNames
            .Where(name => !existingCategoriesNames.Contains(name))
            .Select(name => new Category { Name = name });

        return existingCategories.Concat(newCategories).ToList();
    }

    public async Task RemoveUnusedAsync(Quiz quiz, IEnumerable<string> requestCategories)
    {
        var categoriesToRemove = quiz.Categories
            .Where(c => !requestCategories.Contains(c.Name))
            .ToList();

        foreach (var categoryToRemove in categoriesToRemove)
        {
            quiz.Categories.Remove(categoryToRemove);

            if (await _categoriesRepository.IsAssignedToSingleQuiz(categoryToRemove.Id))
                _repository.RemoveAsync(categoryToRemove);
        }
    }
}

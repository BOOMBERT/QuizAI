using Microsoft.EntityFrameworkCore;
using QuizAI.Domain.Entities;
using QuizAI.Domain.Repositories;
using QuizAI.Infrastructure.Persistence;

namespace QuizAI.Infrastructure.Repositories;

public class CategoriesRepository : ICategoriesRepository
{
    private readonly AppDbContext _context;

    public CategoriesRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Category>> GetExistingCategoriesAsync(IEnumerable<string> categoriesNames)
    {
        return await _context.Categories
            .Where(c => categoriesNames.Contains(c.Name))
            .ToArrayAsync();
    }
}

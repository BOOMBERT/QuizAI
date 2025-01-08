using Microsoft.EntityFrameworkCore;
using QuizAI.Domain.Constants;
using QuizAI.Domain.Entities;
using QuizAI.Domain.Repositories;
using QuizAI.Infrastructure.Persistence;
using System.Linq.Expressions;

namespace QuizAI.Infrastructure.Repositories;

public class QuizzesRepository : IQuizzesRepository
{
    private readonly AppDbContext _context;

    public QuizzesRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Quiz?> GetWithCategoriesAsync(Guid quizId)
    {
        return await _context.Quizzes
            .Include(qz => qz.Categories)
            .FirstOrDefaultAsync(qz => qz.Id == quizId);
    }

    public async Task<Guid?> GetImageIdAsync(Guid quizId)
    {
        return await _context.Quizzes
            .Where(qz => qz.Id == quizId)
            .Select(qz => qz.ImageId)
            .FirstOrDefaultAsync();
    }

    public async Task<(IEnumerable<Quiz>, int)> GetAllMatchingAsync(
        string? searchPhrase,
        int pageSize,
        int pageNumber,
        string? sortBy,
        SortDirection? sortDirection,
        ICollection<string> FilterCategories
        )
    {
        var baseQuery = _context.Quizzes.
            Include(qz => qz.Categories)
            .AsQueryable();

        if (FilterCategories.Count > 0)
            baseQuery = baseQuery
                .Where(qz => qz.Categories
                .Any(c => FilterCategories.Select(c => c.ToLower()).Contains(c.Name)));

        if (!string.IsNullOrEmpty(searchPhrase))
        {
            var searchPhraseLower = searchPhrase.ToLower();

            baseQuery = baseQuery.Where(
                qz => qz.Name.ToLower().Contains(searchPhraseLower) ||
                (qz.Description != null && qz.Description.ToLower().Contains(searchPhraseLower))
            );
        }

        var totalCount = await baseQuery.CountAsync();

        if (!string.IsNullOrEmpty(sortBy) && sortDirection != null)
        {
            var columnsSelector = new Dictionary<string, Expression<Func<Quiz, object>>>(StringComparer.OrdinalIgnoreCase)
            {
                { nameof(Quiz.Name), qz => qz.Name },
                { nameof(Quiz.Description), qz => qz.Description ?? "Z"},
                { nameof(Quiz.CreationDate), qz => qz.CreationDate },
            };

            if (columnsSelector.ContainsKey(sortBy))
            {
                var selectedColumn = columnsSelector[sortBy];

                baseQuery = sortDirection == SortDirection.Ascending
                    ? baseQuery.OrderBy(selectedColumn)
                    : baseQuery.OrderByDescending(selectedColumn);
            }
        }

        var quizzes = await baseQuery
            .Skip(pageSize * (pageNumber - 1))
            .Take(pageSize)
            .ToListAsync();

        return (quizzes, totalCount);
    }

    public async Task<ICollection<Question>> GetQuestionsAsync(Guid quizId)
    {
        return await _context.Quizzes
            .Where(qz => qz.Id == quizId)
            .Include(qz => qz.Questions)
            .Select(qz => qz.Questions)
            .FirstOrDefaultAsync() ?? new List<Question>();
    }

    public async Task<ICollection<Question>> GetQuestionsWithAnswersAsync(Guid quizId)
    {
        return await _context.Questions
            .Include(qn => qn.MultipleChoiceAnswers)
            .Include(qn => qn.OpenEndedAnswer)
            .Include(qn => qn.TrueFalseAnswer)
            .Where(qn => qn.QuizId == quizId)
            .ToListAsync();
    }

    public async Task UpdateImageAsync(Guid quizId, Guid? imageId)
    {
        await _context.Quizzes
            .Where(qz => qz.Id == quizId)
            .ExecuteUpdateAsync(qz => qz.SetProperty(x => x.ImageId, imageId));
    }

    public async Task<IEnumerable<Guid>> GetQuestionsImagesNamesAsync(Guid quizId)
    {
        return await _context.Questions
            .Where(qn => qn.QuizId == quizId && qn.ImageId != null)
            .Select(qn => (Guid)qn.ImageId!)
            .ToArrayAsync();
    }

    public async Task<int> HowManyQuestionsAsync(Guid quizId)
    {
        return await _context.Questions
            .CountAsync(qn => qn.QuizId == quizId);
    }
}

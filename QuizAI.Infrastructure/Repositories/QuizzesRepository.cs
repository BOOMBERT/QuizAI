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

    public async Task<Quiz?> GetAsync(Guid quizId, bool includeCategories = false, bool includeQuestionsWithAnswers = false)
    {
        var baseQuery = _context.Quizzes.AsQueryable();

        if (includeCategories)
            baseQuery = baseQuery.Include(qz => qz.Categories);
        
        if (includeQuestionsWithAnswers)
        {
            baseQuery = baseQuery
                .Include(qz => qz.Questions)
                    .ThenInclude(qn => qn.TrueFalseAnswer)
                .Include(qz => qz.Questions)
                    .ThenInclude(qn => qn.MultipleChoiceAnswers)
                .Include(qz => qz.Questions)
                    .ThenInclude(qn => qn.OpenEndedAnswer);
        }

        return await baseQuery.FirstOrDefaultAsync(qz => qz.Id == quizId);
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
        var baseQuery = _context.Quizzes
            .Where(qz => qz.IsDeprecated == false)
            .Include(qz => qz.Categories)
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

    public async Task<QuizAttempt?> GetUnfinishedQuizAttemptAsync(Guid quizId, string userId)
    {
        return await _context.QuizAttempts
            .Where(qa => qa.QuizId == quizId && qa.UserId == userId && qa.FinishedAt == null)
            .FirstOrDefaultAsync();
    }

    public async Task UpdateLatestVersionIdAsync(Guid oldLatestVersionId, Guid newLatestVersionId)
    {
        await _context.Quizzes
            .Where(qz => qz.LatestVersionId == oldLatestVersionId)
            .ExecuteUpdateAsync(qz => qz.SetProperty(x => x.LatestVersionId, newLatestVersionId));
    }
}

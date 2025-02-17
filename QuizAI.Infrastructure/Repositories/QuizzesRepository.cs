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

    public async Task<Quiz?> GetAsync(Guid quizId, bool includeCategories = false, bool includeQuestionsWithAnswers = false, bool trackChanges = true)
    {
        var baseQuery = _context.Quizzes.AsQueryable();

        if (!trackChanges) baseQuery = baseQuery.AsNoTracking();

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

    public async Task<Quiz?> GetWithQuestionsAndCategoriesAsync(Guid quizId)
    {
        return await _context.Quizzes
            .Include(qz => qz.Questions)
            .Include(qz => qz.Categories)
            .FirstOrDefaultAsync(qz => qz.Id == quizId);
    }

    public async Task<(IEnumerable<Quiz>, int)> GetAllMatchingAsync(
        string userId,
        string? searchPhrase,
        int pageSize,
        int pageNumber,
        string? sortBy,
        SortDirection? sortDirection,
        bool filterByCreatorId,
        ICollection<string> filterCategories,
        bool filterBySharedQuizzes
        )
    {
        var baseQuery = _context.Quizzes
            .AsNoTracking()
            .Include(qz => qz.Categories)
            .Include(qz => qz.QuizPermissions)
            .Where(qz => qz.IsDeprecated == false);

        if (filterBySharedQuizzes || filterByCreatorId)
        {
            if (filterBySharedQuizzes)
            {
                baseQuery = baseQuery
                    .Where(qz => _context.QuizPermissions.Any(qp => qp.QuizId == qz.Id && qp.UserId == userId));
            }
            else if (filterByCreatorId)
            {
                baseQuery = baseQuery
                    .Where(qz => qz.CreatorId == userId);
            }
        }
        else
        {
            baseQuery = baseQuery
                .Where(qz => 
                qz.IsPrivate == false || 
                (qz.IsPrivate == true && qz.CreatorId == userId) || 
                _context.QuizPermissions.Any(qp => qp.QuizId == qz.Id && qp.UserId == userId)
                );
        }

        if (filterCategories.Count > 0)
            baseQuery = baseQuery
                .Where(qz => qz.Categories
                .Any(c => filterCategories.Select(c => c.ToLower()).Contains(c.Name)));


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

    public async Task<(string, int)?> GetNameAndQuestionCountAsync(Guid quizId)
    {
        var result = await _context.Quizzes
            .Where(qz => qz.Id == quizId)
            .Select(qz => new { qz.Name, qz.QuestionCount })
            .FirstOrDefaultAsync();

        if (result == null) return null;

        return (result.Name, result.QuestionCount);
    }

    public async Task<(string, bool, bool, Guid?)?> GetCreatorIdAndIsPrivateAndIsDeprecatedAndLatestVersionIdAsync(Guid quizId)
    {
        var result = await _context.Quizzes
            .Where(qz => qz.Id == quizId)
            .Select(qz => new { qz.CreatorId, qz.IsPrivate, qz.IsDeprecated, qz.LatestVersionId })
            .FirstOrDefaultAsync();

        if (result == null) return null;

        return (result.CreatorId, result.IsPrivate, result.IsDeprecated, result.LatestVersionId);
    }

    public async Task<(string, bool, Guid?)?> GetCreatorIdAndIsDeprecatedAndLatestVersionIdAsync(Guid quizId)
    {
        var result = await _context.Quizzes
            .Where(qz => qz.Id == quizId)
            .Select(qz => new { qz.CreatorId, qz.IsDeprecated, qz.LatestVersionId })
            .FirstOrDefaultAsync();

        if (result == null) return null;

        return (result.CreatorId, result.IsDeprecated, result.LatestVersionId);
    }

    public async Task UpdateLatestVersionIdAsync(Guid oldLatestVersionId, Guid? newLatestVersionId)
    {
        await _context.Quizzes
            .Where(qz => qz.LatestVersionId == oldLatestVersionId)
            .ExecuteUpdateAsync(qz => qz.SetProperty(x => x.LatestVersionId, newLatestVersionId));
    }
}

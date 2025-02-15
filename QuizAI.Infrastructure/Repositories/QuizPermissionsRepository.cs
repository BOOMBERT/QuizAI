using Microsoft.EntityFrameworkCore;
using QuizAI.Domain.Entities;
using QuizAI.Domain.Repositories;
using QuizAI.Infrastructure.Persistence;

namespace QuizAI.Infrastructure.Repositories;

public class QuizPermissionsRepository : IQuizPermissionsRepository
{
    private readonly AppDbContext _context;

    public QuizPermissionsRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<QuizPermission?> GetAsync(Guid quizId, string userId, bool trackChanges = true)
    {
        var baseQuery = _context.QuizPermissions
            .Where(qp => qp.QuizId == quizId && qp.UserId == userId);

        if (!trackChanges)
            baseQuery = baseQuery.AsNoTracking();

        return await baseQuery.FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<QuizPermission>> GetAllAsync(Guid quizId)
    {
        return await _context.QuizPermissions
            .Where(qp => qp.QuizId == quizId)
            .ToArrayAsync();
    }

    public async Task DeletePermissionsAsync(Guid quizId)
    {
        await _context.QuizPermissions
            .Where(qp => qp.QuizId == quizId)
            .ExecuteDeleteAsync();
    }

    public async Task<bool> CheckAsync(Guid quizId, string userId, bool? canEdit, bool? canPlay)
    {
        return await _context.QuizPermissions
            .AnyAsync(
                qp => qp.QuizId == quizId && qp.UserId == userId && 
                (canEdit == null || qp.CanEdit == canEdit) && 
                (canPlay == null || qp.CanPlay == canPlay));
    }
}

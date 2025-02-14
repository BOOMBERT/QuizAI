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

    public async Task<QuizPermission?> GetAsync(Guid quizId, string userId)
    {
        return await _context.QuizPermissions
            .Where(qp => qp.QuizId == quizId && qp.UserId == userId)
            .FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<QuizPermission>> GetAllAsync(Guid quizId)
    {
        return await _context.QuizPermissions
            .Where(qp => qp.QuizId == quizId)
            .ToArrayAsync();
    }

    public async Task UpdateQuizIdAsync(Guid previousQuizId, Guid newQuizId)
    {
        var quizPermissions = await _context.QuizPermissions
            .Where(qp => qp.QuizId == previousQuizId)
            .ToListAsync();

        foreach (var quizPermission in quizPermissions)
        {
            quizPermission.QuizId = newQuizId;
        }
    }

    public async Task DeletePermissionsAsync(Guid quizId)
    {
        await _context.QuizPermissions
            .Where(qp => qp.QuizId == quizId)
            .ExecuteDeleteAsync();
    }

    public async Task<bool> HasAnyAsync(Guid quizId, string userId)
    {
        return await _context.QuizPermissions
            .AnyAsync(qp => qp.QuizId == quizId && qp.UserId == userId);
    }

    public async Task<bool> CanEditAsync(Guid quizId, string userId)
    {
        return await _context.QuizPermissions
            .AnyAsync(qp => qp.QuizId == quizId && qp.UserId == userId && qp.CanEdit == true);
    }
}

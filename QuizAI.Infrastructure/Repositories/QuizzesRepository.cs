using Microsoft.EntityFrameworkCore;
using QuizAI.Domain.Entities;
using QuizAI.Domain.Repositories;
using QuizAI.Infrastructure.Persistence;

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

    public async Task<Guid?> GetImageId(Guid quizId)
    {
        return await _context.Quizzes
            .Where(qz => qz.Id == quizId)
            .Select(qz => qz.ImageId)
            .FirstOrDefaultAsync();
    }
}

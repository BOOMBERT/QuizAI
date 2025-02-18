using Microsoft.EntityFrameworkCore;
using QuizAI.Domain.Entities;
using QuizAI.Domain.Repositories;
using QuizAI.Infrastructure.Persistence;

namespace QuizAI.Infrastructure.Repositories;

public class ImagesRepository : IImagesRepository
{
    private readonly AppDbContext _context;

    public ImagesRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Image?> GetAsync(byte[] hash)
    {
        return await _context.Images
            .Include(i => i.Quizzes)
            .Where(i => i.Hash.SequenceEqual(hash))
            .FirstOrDefaultAsync();
    }

    public async Task<bool> IsInStorageAsync(byte[] hash, bool privateStorage)
    {
        return await _context.Images
            .Include(i => i.Quizzes)
            .AnyAsync(i => i.Quizzes.Any(qz => qz.IsPrivate == privateStorage));
    }

    public async Task<bool> IsAssignedToAnyQuizAsync(Guid imageId, Guid? quizIdToSkip, bool? onlyPrivate = null)
    {
        var baseQuery = _context.Quizzes
            .AsQueryable();

        if (onlyPrivate != null)
        {
            baseQuery = baseQuery
                .Where(qz => qz.IsPrivate == onlyPrivate); 
        }

        if (quizIdToSkip == null)
        {
            return await baseQuery
                .AnyAsync(qz => qz.ImageId == imageId);
        }

        return await baseQuery
                .AnyAsync(qz => qz.ImageId == imageId && qz.Id != quizIdToSkip);
    }

    public async Task<bool> IsAssignedToAnyQuestionAsync(Guid imageId, int? questionIdToSkip, bool? onlyPrivate = null)
    {
        var baseQuery = _context.Questions
            .AsQueryable();

        if (onlyPrivate != null)
        {
            baseQuery = baseQuery
                .Include(qn => qn.Quiz)
                .Where(qn => qn.Quiz.IsPrivate == onlyPrivate);
        }

        if (questionIdToSkip == null)
        {
            return await baseQuery
                .AnyAsync(qn => qn.ImageId == imageId);
        }

        return await baseQuery
            .AnyAsync(qn => qn.ImageId == imageId && qn.Id != questionIdToSkip);
    }

    public async Task<IEnumerable<Image>> GetQuizAndItsQuestionImagesAsync(Guid quizId, Guid? quizImageId)
    {
        var questionImagesIds = _context.Questions
            .Where(qn => qn.QuizId == quizId && qn.ImageId != null).Select(qn => qn.ImageId).ToArray();

        if (quizImageId == null && questionImagesIds.Length == 0)
            return Enumerable.Empty<Image>();

        var baseQuery = _context.Images
            .AsNoTracking();

        if (quizImageId != null)
            return await baseQuery.Where(i => i.Id == quizImageId || questionImagesIds.Contains(i.Id)).ToArrayAsync();

        return await baseQuery.Where(i => questionImagesIds.Contains(i.Id)).ToArrayAsync();
    }

    public async Task<string?> GetFileExtensionAsync(Guid imageId)
    {
        return await _context.Images
            .Where(i => i.Id == imageId)
            .Select(i => i.FileExtension)
            .FirstOrDefaultAsync();
    }
}
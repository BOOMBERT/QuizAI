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
            .Where(i => i.Hash.SequenceEqual(hash))
            .FirstOrDefaultAsync();
    }

    public async Task<string?> GetExtensionAsync(Guid id)
    {
        return await _context.Images
            .Where(i => i.Id == id)
            .Select(i => i.FileExtension)
            .FirstOrDefaultAsync();
    }
}

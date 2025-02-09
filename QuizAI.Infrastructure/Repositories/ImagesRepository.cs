﻿using Microsoft.EntityFrameworkCore;
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

    public async Task<bool> IsAssignedToAnyQuizAsync(Guid imageId, Guid? quizIdToSkip)
    {
        if (quizIdToSkip == null)
        {
            return await _context.Quizzes
                .AnyAsync(qz => qz.ImageId == imageId);
        }

        return await _context.Quizzes
                .AnyAsync(qz => qz.ImageId == imageId && qz.Id != quizIdToSkip);
    }

    public async Task<bool> IsAssignedToAnyQuestionAsync(Guid imageId, int? questionIdToSkip)
    {
        if (questionIdToSkip == null)
        {
            return await _context.Questions
                .AnyAsync(qn => qn.ImageId == imageId);
        }

        return await _context.Questions
            .AnyAsync(qn => qn.ImageId == imageId && qn.Id != questionIdToSkip);
    }
}
﻿using QuizAI.Domain.Constants;
using QuizAI.Domain.Entities;

namespace QuizAI.Domain.Repositories;

public interface IQuizzesRepository
{
    Task<Quiz?> GetAsync(Guid quizId, bool includeCategories = false, bool includeQuestionsWithAnswers = false);
    Task<(IEnumerable<Quiz>, int)> GetAllMatchingAsync(
        string? searchPhrase,
        int pageSize,
        int pageNumber,
        string? sortBy,
        SortDirection? sortDirection,
        ICollection<string> FilterCategories);
    Task UpdateImageAsync(Guid quizId, Guid? imageId);
    Task UpdateLatestVersionIdAsync(Guid oldLatestVersionId, Guid newLatestVersionId);
}

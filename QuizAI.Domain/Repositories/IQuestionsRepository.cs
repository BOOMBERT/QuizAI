﻿using QuizAI.Domain.Entities;
using QuizAI.Domain.Enums;

namespace QuizAI.Domain.Repositories;

public interface IQuestionsRepository
{
    Task<ICollection<Question>> GetAllAsync(Guid quizId, bool answers = false);
    Task<Question?> GetByOrderAsync(Guid quizId, int order);
    Task<IEnumerable<string>> GetMultipleChoiceAnswersContentAsync(int questionId);
    Task<Guid?> GetImageIdAsync(Guid quizId, int questionId);
    Task<int> HowManyAsync(Guid quizId);
}

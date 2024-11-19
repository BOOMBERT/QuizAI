﻿namespace QuizAI.Domain.Repositories;

public interface IRepository
{
    Task AddAsync<T>(T entity) where T : class;
    Task<bool> SaveChangesAsync();
}

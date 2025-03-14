﻿namespace QuizAI.Domain.Repositories;

public interface IRepository
{
    Task AddAsync<T>(T entity) where T : class;
    Task AddRangeAsync<T>(IEnumerable<T> entities) where T : class;
    Task<TField?> GetFieldAsync<TEntity, TField>(Guid id, string fieldName) where TEntity : class;
    Task<TEntity?> GetEntityAsync<TEntity>(Guid id, bool asNoTracking = true) where TEntity : class;
    Task<TEntity?> GetEntityAsync<TEntity>(int id, bool trackChanges = true) where TEntity : class;
    Task DeleteAsync<T>(Guid id) where T : class;
    void Remove<T>(T entity) where T : class;
    void RemoveRange<T>(IEnumerable<T> entities) where T : class;
    Task<bool> SaveChangesAsync();
}

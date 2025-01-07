namespace QuizAI.Domain.Repositories;

public interface IRepository
{
    Task AddAsync<T>(T entity) where T : class;
    Task AddRangeAsync<T>(IEnumerable<T> entities) where T : class;
    Task<bool> EntityExistsAsync<T>(Guid id) where T : class;
    Task<bool> EntityExistsAsync<T>(int id) where T : class;
    Task DeleteAsync<T>(Guid id) where T : class;
    Task DeleteAsync<T>(int id) where T : class;
    void Remove<T>(T entity) where T : class;
    void RemoveRange<T>(IEnumerable<T> entities) where T : class;
    Task<bool> SaveChangesAsync();
}

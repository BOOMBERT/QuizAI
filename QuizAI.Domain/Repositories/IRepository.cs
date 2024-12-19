namespace QuizAI.Domain.Repositories;

public interface IRepository
{
    Task AddAsync<T>(T entity) where T : class;
    Task<bool> EntityExistsAsync<T>(Guid id) where T : class;
    void RemoveAsync<T>(T entity) where T : class;
    Task<bool> SaveChangesAsync();
}

using Microsoft.EntityFrameworkCore;
using QuizAI.Domain.Repositories;
using QuizAI.Infrastructure.Persistence;

namespace QuizAI.Infrastructure.Repositories;

public class Repository : IRepository
{
    private readonly AppDbContext _context;

    public Repository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync<T>(T entity) where T : class
    {
        await _context.Set<T>().AddAsync(entity);
    }

    public async Task AddRangeAsync<T>(IEnumerable<T> entities) where T : class
    {
        await _context.Set<T>().AddRangeAsync(entities);
    }

    public async Task<TField?> GetFieldAsync<TEntity, TField>(Guid id, string fieldName) where TEntity : class
    {
        return await _context.Set<TEntity>()
            .Where(e => EF.Property<Guid>(e, "Id") == id)
            .Select(e => EF.Property<TField>(e, fieldName))
            .FirstOrDefaultAsync();
    }

    public async Task<TEntity?> GetEntityAsync<TEntity>(Guid id, bool trackChanges = true) where TEntity : class
    {
        var baseQuery = _context.Set<TEntity>().Where(e => EF.Property<Guid>(e, "Id") == id);

        if (!trackChanges)
            baseQuery = baseQuery.AsNoTracking();

        return await baseQuery.FirstOrDefaultAsync();
    }

    public async Task<TEntity?> GetEntityAsync<TEntity>(int id, bool trackChanges = true) where TEntity : class
    {
        var baseQuery = _context.Set<TEntity>().Where(e => EF.Property<int>(e, "Id") == id);

        if (!trackChanges)
            baseQuery = baseQuery.AsNoTracking();

        return await baseQuery.FirstOrDefaultAsync();
    }

    public async Task DeleteAsync<T>(Guid id) where T : class
    {
        await _context.Set<T>()
            .Where(e => EF.Property<Guid>(e, "Id") == id)
            .ExecuteDeleteAsync();
    }

    public void Remove<T>(T entity) where T : class
    {
        _context.Set<T>().Remove(entity);
    }

    public void RemoveRange<T>(IEnumerable<T> entities) where T : class
    {
        _context.Set<T>().RemoveRange(entities);
    }

    public async Task<bool> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync() > 0;
    }
}

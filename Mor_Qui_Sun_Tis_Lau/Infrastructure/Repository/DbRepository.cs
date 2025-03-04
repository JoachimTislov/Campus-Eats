using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Mor_Qui_Sun_Tis_Lau.SharedKernel;

namespace Mor_Qui_Sun_Tis_Lau.Infrastructure.Repository;

public class DbRepository<T>(ShopContext db) : IDbRepository<T> where T : BaseEntity
{
    private readonly ShopContext _db = db ?? throw new ArgumentNullException(nameof(db));

    private DbSet<T> GetEntities()
    {
        return _db.Set<T>();
    }

    private async Task Save() => await _db.SaveChangesAsync();

    public async Task AddAsync(T entity)
    {
        await GetEntities().AddAsync(entity);
        await Save();
    }

    public async Task Remove<E>(E id)
    {
        var entity = await GetByIdAsync(id);
        if (entity != null)
        {
            GetEntities().Remove(entity);
            await Save();
        }
    }

    public async Task RemoveRange(List<T> entities)
    {
        GetEntities().RemoveRange(entities);
        await Save();
    }

    public async Task Update(T entity)
    {
        GetEntities().Update(entity);
        await Save();
    }

    public async Task UpdateRange(List<T> entities)
    {
        GetEntities().UpdateRange(entities);
        await Save();
    }

    public async Task<T?> GetByIdAsync<E>(E id)
    {
        return await _db.FindAsync<T>(id);
    }

    public List<T> All()
    {
        return [.. GetEntities()];
    }

    public async Task<bool> AnyAsync()
    {
        return await GetEntities().AnyAsync();
    }

    public async Task<List<T>> WhereToListAsync(Expression<Func<T, bool>> whereExp)
    {
        return await Where(whereExp).ToListAsync();
    }

    public async Task<List<T>> IncludeToListAsync<I>(Expression<Func<T, I>> include)
    {
        return await Include(include).ToListAsync();
    }

    public async Task<T?> WhereFirstOrDefaultAsync(Expression<Func<T, bool>> whereExp)
    {
        return await Where(whereExp).FirstOrDefaultAsync();
    }

    public async Task<T?> IncludeWhereFirstOrDefaultAsync<I>(Expression<Func<T, I>> includeExp, Expression<Func<T, bool>> whereExp)
    {
        return await Include(includeExp).Where(whereExp).FirstOrDefaultAsync();
    }

    public async Task<List<T>> IncludeOrderByToListAsync<I, O>(Expression<Func<T, I>> includeExp, Expression<Func<T, O>> orderByExp)
    {
        return await Include(includeExp).OrderBy(orderByExp).ToListAsync();
    }

    public async Task<List<T>> IncludeWhereToListAsync<I>(Expression<Func<T, I>> includeExp, Expression<Func<T, bool>> whereExp)
    {
        return await Include(includeExp).Where(whereExp).ToListAsync();
    }

    public async Task<List<T>> WhereOrderByToListAsync<O>(Expression<Func<T, bool>> whereExp, Expression<Func<T, O>> orderByExp)
    {
        return await Where(whereExp).OrderBy(orderByExp).ToListAsync();
    }

    public async Task<List<T>> WhereOrderByDescendingToListAsync<O>(Expression<Func<T, bool>> whereExp, Expression<Func<T, O>> orderByExp)
    {
        return await Where(whereExp).OrderByDescending(orderByExp).ToListAsync();
    }

    public async Task<List<T>> IncludeWhereOrderByToListAsync<I, O>(Expression<Func<T, I>> includeExp, Expression<Func<T, bool>> whereExp, Expression<Func<T, O>> orderByExp)
    {
        return await Include(includeExp).Where(whereExp).OrderBy(orderByExp).ToListAsync();
    }

    private IQueryable<T> Where(Expression<Func<T, bool>> whereExp, IQueryable<T>? entities = null)
    {
        entities ??= GetEntities();
        return entities.Where(whereExp);
    }

    private IQueryable<T> Include<I>(Expression<Func<T, I>> includeExp, IQueryable<T>? entities = null)
    {
        entities ??= GetEntities();
        return entities.Include(includeExp);
    }
}
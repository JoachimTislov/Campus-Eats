using System.Linq.Expressions;
using Mor_Qui_Sun_Tis_Lau.SharedKernel;

namespace Mor_Qui_Sun_Tis_Lau.Infrastructure.Repository;

public interface IDbRepository<T> where T : BaseEntity
{
    Task AddAsync(T entity);
    Task Remove<E>(E id);
    Task RemoveRange(List<T> entities);
    Task Update(T entity);
    Task UpdateRange(List<T> entities);
    Task<T?> GetByIdAsync<E>(E id);
    List<T> All();
    Task<bool> AnyAsync();
    Task<List<T>> WhereToListAsync(Expression<Func<T, bool>> whereExp);
    Task<List<T>> IncludeToListAsync<I>(Expression<Func<T, I>> include);
    Task<T?> WhereFirstOrDefaultAsync(Expression<Func<T, bool>> whereExp);
    Task<T?> IncludeWhereFirstOrDefaultAsync<I>(Expression<Func<T, I>> includeExp, Expression<Func<T, bool>> whereExp);
    Task<List<T>> IncludeOrderByToListAsync<I, O>(Expression<Func<T, I>> includeExp, Expression<Func<T, O>> orderByExp);
    Task<List<T>> IncludeWhereToListAsync<I>(Expression<Func<T, I>> includeExp, Expression<Func<T, bool>> whereExp);
    Task<List<T>> WhereOrderByToListAsync<O>(Expression<Func<T, bool>> whereExp, Expression<Func<T, O>> orderByExp);
    Task<List<T>> WhereOrderByDescendingToListAsync<O>(Expression<Func<T, bool>> whereExp, Expression<Func<T, O>> orderByExp);
    Task<List<T>> IncludeWhereOrderByToListAsync<I, O>(Expression<Func<T, I>> includeExp, Expression<Func<T, bool>> whereExp, Expression<Func<T, O>> orderByExp);
}
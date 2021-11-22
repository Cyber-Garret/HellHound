using System.Linq.Expressions;

using Hound.Domain.SeedWork;

using Microsoft.EntityFrameworkCore;

namespace Hound.Infrastructure;

internal abstract class RepositoryBase<T> : IRepositoryBase<T> where T : class
{
	private HoundContext RepositoryContext { get; }

	protected RepositoryBase(HoundContext repositoryContext)
	{
		RepositoryContext = repositoryContext;
	}

	public IQueryable<T> FindAll(bool trackChanges = false) =>
		trackChanges
			? RepositoryContext.Set<T>()
			: RepositoryContext.Set<T>().AsNoTracking();
	public IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression, bool trackChanges = false) =>
		trackChanges
			? RepositoryContext.Set<T>().Where(expression)
			: RepositoryContext.Set<T>().Where(expression).AsNoTracking();


	public async Task Create(T entity) =>
		await RepositoryContext.Set<T>().AddAsync(entity);

	public void Update(T entity) =>
		RepositoryContext.Set<T>().Update(entity);

	public void Delete(T entity) =>
		RepositoryContext.Set<T>().Remove(entity);
}
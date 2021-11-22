using System.Linq.Expressions;

namespace Hound.Domain.SeedWork;

public interface IRepositoryBase<T>
{
	IQueryable<T> FindAll(bool trackChanges = false);
	IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression, bool trackChanges = false);
	Task Create(T entity);
	void Update(T entity);
	void Delete(T entity);
}
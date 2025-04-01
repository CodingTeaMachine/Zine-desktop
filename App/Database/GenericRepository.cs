using System.Data;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace Zine.App.Database;

public sealed class GenericRepository<TEntity>(ZineDbContext context) where TEntity : class
{
	private readonly DbSet<TEntity> _dbSet = context.Set<TEntity>();

	public int Count(Expression<Func<TEntity, bool>>? filter = null)
	{
		return filter != null
			? _dbSet.Count(filter)
			: _dbSet.Count();
	}

	public IEnumerable<TEntity> List(
		Expression<Func<TEntity, bool>>? filter = null,
		Expression<Func<TEntity, object>>? orderBy = null,
		Expression<Func<TEntity, object>>? orderByDescending = null,
		Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? includes = null,
		int? skip = null,
		int? take = null)
	{
		IQueryable<TEntity> query = _dbSet;

		if (filter != null)
			query = query.Where(filter);

		if(skip != null)
			query = query.Skip(skip.Value);

		if(take != null)
			query = query.Take(take.Value);

		if (includes != null)
			query = includes(query);

		if(orderBy != null)
			query = query.OrderBy(orderBy);

		if(orderByDescending != null)
			query = query.OrderByDescending(orderByDescending);

		return query.ToList();
	}

	public IEnumerable<TEntity> List(
		ASearchDto<TEntity> searchQuery,
		Expression<Func<TEntity, object>>? orderBy = null,
		Expression<Func<TEntity, object>>? orderByDescending = null,
		Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? includes = null,
		int? skip = null,
		int? take = null)
	{
		IQueryable<TEntity> query = _dbSet;

		query = searchQuery.ToServerQuery(query);

		if(skip != null)
			query = query.Skip(skip.Value);

		if(take != null)
			query = query.Take(take.Value);

		if (includes != null)
			query = includes(query);

		if(orderBy != null)
			query = query.OrderBy(orderBy);

		if(orderByDescending != null)
			query = query.OrderByDescending(orderByDescending);

		var result = query.ToList();

		return searchQuery.ToClientQuery(result);
	}

	public TEntity? First(
		Expression<Func<TEntity, bool>> filter,
		Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? includes = null)
	{

		IQueryable<TEntity> query = _dbSet;

		if (includes != null)
		{
			query = includes(query);
		}

		return query.FirstOrDefault(filter);
	}

	/// <summary>
	///
	/// </summary>
	/// <param name="id"></param>
	/// <returns></returns>
	/// <exception cref="DataException"></exception>
	public TEntity? GetById(int id)
	{
		return _dbSet.Find(id);
	}

	public void Insert(TEntity entity)
	{
		_dbSet.Add(entity);
	}

	public void InsertMany(IEnumerable<TEntity> entities)
	{
		_dbSet.AddRange(entities);
	}

	public void Update(TEntity entity)
	{
		_dbSet.Attach(entity);
		context.Entry(entity).State = EntityState.Modified;
	}

	public void Delete(int id)
	{
		TEntity? entity = _dbSet.Find(id);

		if (entity == null)
		{
			throw new DataException("Could not find entity with id: " + id);
		}

		Delete(entity);
	}

	public void Delete(TEntity entity)
	{
		if (context.Entry(entity).State == EntityState.Detached)
		{
			_dbSet.Attach(entity);
		}
		_dbSet.Remove(entity);
	}


}

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace EasyDdd.Kernel
{
	public class DbContextRepository<TContext, TEntity> : IRepository<TEntity>
		where TContext : DbContext
		where TEntity : class
	{
		private readonly TContext _context;

		public DbContextRepository(TContext context)
		{
			_context = context;
		}

		public async Task<IReadOnlyList<TEntity>> FindAsync(Specification<TEntity> specification)
		{
			var results = await _context.Set<TEntity>()
				.AsTracking()
				.Where(specification.ToExpression())
				.ToListAsync();

			return results;
		}

		public async Task SaveAsync(TEntity entity)
		{
			var state = _context.Entry(entity)?.State ?? EntityState.Detached;

			if (state == EntityState.Detached)
				_context.Add(entity);
			else
				_context.Update(entity);

			await _context.SaveChangesAsync();
		}

		public async Task DeleteAsync(TEntity entity)
		{
			_context.Remove(entity);
			await _context.SaveChangesAsync();
		}
	}
}
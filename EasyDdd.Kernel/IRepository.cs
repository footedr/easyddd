using System.Collections.Generic;
using System.Threading.Tasks;

namespace EasyDdd.Kernel
{
	public interface IRepository<TEntity>
	{
		Task<IReadOnlyList<TEntity>> FindAsync(Specification<TEntity> specification);
		Task SaveAsync(TEntity entity);
		Task DeleteAsync(TEntity entity);
	}
}
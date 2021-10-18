using System.Linq;
using System.Security.Claims;

namespace EasyDdd.Kernel
{
	public interface IReadModel<T>
	{
		IQueryable<T> Query(ClaimsPrincipal user);
	}
}
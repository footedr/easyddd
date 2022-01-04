using System.Collections.Generic;
using System.Threading.Tasks;

namespace EasyDdd.Kernel;

public static class TaskExtensions
{
	public static async Task<T?> SingleOrDefaultAsync<T>(this Task<IReadOnlyList<T>> task)
		where T : class
	{
		var results = await task;
		return results.Count != 1 ? default : results[0];
	}
}
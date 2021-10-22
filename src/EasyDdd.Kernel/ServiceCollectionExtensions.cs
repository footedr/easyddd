using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EasyDdd.Kernel
{
	public static class ServiceCollectionExtensions
	{
		public static IServiceCollection AddRepository<TEntity, TContext>(this IServiceCollection services)
			where TEntity : class
			where TContext : DbContext
		{
			services.AddTransient<IRepository<TEntity>, DbContextRepository<TContext, TEntity>>();

			return services;
		}
	}
}
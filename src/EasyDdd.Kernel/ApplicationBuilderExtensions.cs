using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.AspNetCore.Builder;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EasyDdd.Kernel;

public static class ApplicationBuilderExtensions
{
	/// <summary>
	///     Migrates the database associated with the given DbContext to the latest version.  Will retry 5 times
	///     waiting 5 seconds between attempts before throwing an <see cref="AggregateException" /> with the
	///     exception details from all attempts.
	/// </summary>
	/// <typeparam name="TContext"></typeparam>
	/// <param name="app"></param>
	/// <returns></returns>
	public static IApplicationBuilder MigrateDatabase<TContext>(this IApplicationBuilder app) where TContext : DbContext
	{
		// When running in docker, there is a delay between when the SQL Server container is started,
		// and when the container is ready to accept connections.  Here we try a few times with a delay between
		// each attempt to give SQL Server enough time to warm up.
		var errors = new List<Exception>();
		var migrationComplete = false;

		for (var attempt = 1; attempt <= 5; attempt++)
			try
			{
				using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
				{
					Console.WriteLine($"Attempt {attempt} to migrate database");

					using (var context = serviceScope.ServiceProvider.GetRequiredService<TContext>())
					{
						context.Database.Migrate();
					}

					Console.WriteLine("Migration complete");
					migrationComplete = true;
					break;
				}
			}
			catch (SqlException ex)
			{
				errors.Add(ex);
				Thread.Sleep(5000);
			}

		if (!migrationComplete) throw new AggregateException("Unable to complete database migration", errors);

		return app;
	}
}
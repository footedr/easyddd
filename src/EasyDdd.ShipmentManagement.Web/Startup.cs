using System.Security.Claims;
using System.Text.Json;
using EasyDdd.Kernel;
using EasyDdd.Kernel.EventGrid;
using EasyDdd.ShipmentManagement.Core;
using EasyDdd.ShipmentManagement.Data;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace EasyDdd.ShipmentManagement.Web;

public class Startup
{
	public IConfiguration EventGridConfiguration { get; }

	public Startup(IConfiguration configuration)
	{
		Configuration = configuration;
		EventGridConfiguration = Configuration.GetSection("EventGrid");
	}

	public IConfiguration Configuration { get; }

	// This method gets called by the runtime. Use this method to add services to the container.
	public void ConfigureServices(IServiceCollection services)
	{
		services.AddMediatR(typeof(Shipment), typeof(TmsContext));
		services.AddDbContext<TmsContext>(opt => { opt.UseSqlServer(Configuration["TmsDb"], sql => { sql.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery); }); });
		services.AddRepository<Shipment, TmsContext>();
		services.AddTransient<IReadModel<Shipment>, ShipmentsReadModel>();
		services.AddTransient<IDispatchNumberService, DispatchNumberService>();
		services.AddScoped<NodaTime.IClock>(_ => NodaTime.SystemClock.Instance);
		services.AddScoped<IClock, SystemClock>();
		services.AddRazorPages();
		services.AddEventGridDomainEventHandler(
			EventGridConfiguration["Hostname"],
			EventGridConfiguration["Key"],
			jsonOptions: new JsonSerializerOptions());
	}

	// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
	public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
	{
		if (env.IsDevelopment())
		{
			app.MigrateDatabase<TmsContext>();
		}

		app.Use(async (context, next) =>
		{
			context.User = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, "SYSTEM") }));
			await next.Invoke();
		});

		if (env.IsDevelopment())
		{
			app.UseDeveloperExceptionPage();
		}
		else
		{
			app.UseExceptionHandler("/Error");
			// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
			app.UseHsts();
		}

		app.UseHttpsRedirection();
		app.UseStaticFiles();

		app.UseRouting();

		app.UseAuthorization();

		app.UseEndpoints(endpoints => { endpoints.MapRazorPages(); });

		app.UseStatusCodePagesWithRedirects("/errors/{0}");
	}
}
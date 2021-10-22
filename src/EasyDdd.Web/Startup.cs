using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EasyDdd.Core;
using EasyDdd.Data;
using EasyDdd.Kernel;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NodaTime;

namespace EasyDdd.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
			services.AddMediatR(typeof(Shipment), typeof(TmsContext));
			services.AddDbContext<TmsContext>(opt =>
			{
				opt.UseSqlServer(Configuration["TmsDb"], sql =>
				{
					sql.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
				});
			});
			services.AddRepository<Shipment, TmsContext>();
			services.AddTransient<IReadModel<Shipment>, ShipmentsReadModel>();
			services.AddScoped<IClock>(_ => SystemClock.Instance);

			services.AddRazorPages();
		}

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
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

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });
        }
    }
}

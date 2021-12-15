using System.Security.Claims;
using System.Text.Json;
using EasyDdd.Kernel;
using EasyDdd.Kernel.EventGrid;
using EasyDdd.ShipmentManagement.Core;
using EasyDdd.ShipmentManagement.Data;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using IClock = NodaTime.IClock;
using SystemClock = NodaTime.SystemClock;

var builder = WebApplication.CreateBuilder(args);

var eventGridConfig = builder.Configuration.GetSection("EventGrid");

builder.Services.AddMediatR(typeof(Shipment), typeof(TmsContext));
builder.Services.AddDbContext<TmsContext>(opt =>
{
	opt.UseSqlServer(builder.Configuration["TmsDb"], sql =>
	{
		sql.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
	});
});
builder.Services.AddRepository<Shipment, TmsContext>();
builder.Services.AddTransient<IReadModel<Shipment>, ShipmentsReadModel>();
builder.Services.AddTransient<IDispatchNumberService, DispatchNumberService>();
builder.Services.AddTransient<IShipmentIdService, ShipmentIdService>();
builder.Services.AddScoped<IClock>(_ => SystemClock.Instance);
builder.Services.AddScoped<EasyDdd.Kernel.IClock, EasyDdd.Kernel.SystemClock>();
builder.Services.AddRazorPages();
builder.Services.AddEventGridDomainEventHandler(
	eventGridConfig["Hostname"],
	eventGridConfig["Key"],
	jsonOptions: new JsonSerializerOptions());

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
	app.MigrateDatabase<TmsContext>();
}

app.Use(async (context, next) =>
{
	context.User = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, "SYSTEM") }));
	await next.Invoke();
});

if (app.Environment.IsDevelopment())
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

app.MapRazorPages();

app.UseStatusCodePagesWithRedirects("/errors/{0}");

app.Run();
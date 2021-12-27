using System.Text.Json;
using EasyDdd.Billing.Data;
using EasyDdd.Billing.Web.Converters;
using EasyDdd.Billing.Web.Pages;
using EasyDdd.Kernel;
using EasyDdd.Kernel.EventGrid;
using EasyDdd.ShipmentManagement.Core;
using MediatR;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var eventGridConfig = builder.Configuration.GetSection("EventGrid");

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddSingleton<NodaTime.IClock>(NodaTime.SystemClock.Instance);
builder.Services.AddSingleton<IClock>(new SystemClock());
builder.Services.AddMediatR(typeof(ShipmentDelivered), typeof(BillingContext));
builder.Services.AddDbContext<BillingContext>(opt =>
{
	opt.UseSqlServer(builder.Configuration["TmsDb"], sql =>
	{
		sql.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
	});
});
builder.Services.AddRepository<Shipment, BillingContext>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Error");
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}
else
{
	app.MigrateDatabase<BillingContext>();
}

app.UseEventGrid(
	"/api/eventgrid/events",
	eventGridConfig["WebHookApiKey"],
	new JsonSerializerOptions().ConfigureConverters());

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
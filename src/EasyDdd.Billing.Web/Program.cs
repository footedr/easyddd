using System.Text.Json;
using EasyDdd.Billing.Data;
using EasyDdd.Kernel;
using EasyDdd.Kernel.EventGrid;
using EasyDdd.ShipmentManagement.Core;
using MediatR;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var eventGridConfig = builder.Configuration.GetSection("EventGrid");

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddMediatR(typeof(ShipmentDelivered), typeof(BillingContext));
builder.Services.AddDbContext<BillingContext>(opt => { opt.UseSqlServer(builder.Configuration["TmsDb"], sql => { sql.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery); }); });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
	// Todo: Add db migration
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
	new JsonSerializerOptions());

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
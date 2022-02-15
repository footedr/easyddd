using System.Security.Claims;
using System.Text.Json;
using EasyDdd.Billing.Core;
using EasyDdd.Billing.Data;
using EasyDdd.Billing.Web.Converters;
using EasyDdd.Kernel;
using EasyDdd.Kernel.EventGrid;
using EasyDdd.Kernel.EventHubs;
using MediatR;
using Microsoft.EntityFrameworkCore;
using IClock = NodaTime.IClock;
using SystemClock = NodaTime.SystemClock;

var builder = WebApplication.CreateBuilder(args);

var eventGridConfig = builder.Configuration.GetSection("EventGrid");
var eventConsumerConfig = builder.Configuration.GetSection("EventConsumer");

builder.Services.AddMediatR(typeof(Shipment), typeof(BillingContext));
builder.Services.AddDbContext<BillingContext>(opt => { opt.UseSqlServer(builder.Configuration["TmsDb"], sql => { sql.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery); }); });
builder.Services.AddRepository<Shipment, BillingContext>();
builder.Services.AddRepository<Statement, BillingContext>();
builder.Services.AddTransient<IStatementRepository, StatementRepository>();
builder.Services.AddTransient<StatementService>();
builder.Services.AddTransient<IReadModel<Shipment>, ShipmentsReadModel>();
builder.Services.AddTransient<IReadModel<Statement>, StatementsReadModel>();
builder.Services.AddSingleton<IClock>(SystemClock.Instance);
builder.Services.AddSingleton<EasyDdd.Kernel.IClock>(new EasyDdd.Kernel.SystemClock());
builder.Services.AddRazorPages();

builder.Services.Configure<BillingOptions>(builder.Configuration.GetSection(BillingOptions.Billing));

if (builder.Environment.IsDevelopment())
{
	builder.Services.AddDomainEventConsumer(
		new DomainEventConsumerConfiguration("shipments",
			"billing",
			eventConsumerConfig["Endpoint"],
			new JsonSerializerOptions().ConfigureConverters()));
}
else
{
	builder.Services.AddDomainEventConsumer(
		new DomainEventConsumerWithSaslConfiguration("shipments",
			"billing",
			eventConsumerConfig["Endpoint"],
			eventConsumerConfig["ConnectionString"],
			new JsonSerializerOptions().ConfigureConverters()));
}


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
	app.UseDeveloperExceptionPage();
	app.MigrateDatabase<BillingContext>();
}

app.Use(async (context, next) =>
{
	context.User = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, "SYSTEM") }));
	await next.Invoke();
});

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
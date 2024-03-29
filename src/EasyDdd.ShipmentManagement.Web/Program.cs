using System.ComponentModel;
using System.Security.Claims;
using Confluent.Kafka;
using EasyDdd.Kernel;
using EasyDdd.Kernel.Kafka;
using EasyDdd.ShipmentManagement.Core;
using EasyDdd.ShipmentManagement.Data;
using EasyDdd.ShipmentManagement.Web.Converters;
using MediatR;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var kafkaConfiguration = builder.Configuration.GetSection("Kafka");

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
builder.Services.AddSingleton<NodaTime.IClock>(NodaTime.SystemClock.Instance);
builder.Services.AddSingleton<IClock>(new SystemClock());
builder.Services.AddRazorPages();

builder.Services.AddKafkaProducer(options =>
{
    options.ProducerConfig.BootstrapServers = kafkaConfiguration["Endpoint"];

    if (builder.Environment.IsDevelopment())
    {
        options.ProducerConfig.SecurityProtocol = SecurityProtocol.Plaintext;
        return;
    }

    // If you're using Confluent Cloud, for example.
    options.ProducerConfig.SecurityProtocol = SecurityProtocol.SaslSsl;
    options.ProducerConfig.SaslMechanism = SaslMechanism.Plain;
    options.ProducerConfig.SaslUsername = kafkaConfiguration["ApiKey"];     // Set secret during deployment
    options.ProducerConfig.SaslPassword = kafkaConfiguration["ApiSecret"];  // Set secret during deployment
}).AddDomainEventPublisher(options =>
{
    options.JsonSerializerOptions.ConfigureConverters();
});

TypeDescriptor.AddAttributes(typeof(ShipmentId), new TypeConverterAttribute(typeof(ShipmentIdTypeConverter)));

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Error");
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}
else
{
	app.UseDeveloperExceptionPage();
	app.MigrateDatabase<TmsContext>();
}

app.Use(async (context, next) =>
{
	context.User = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, "SYSTEM") }));
	await next.Invoke();
});

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.UseStatusCodePagesWithRedirects("/errors/{0}");

app.Run();
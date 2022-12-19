using System.ComponentModel;
using System.Security.Claims;
using System.Text.Json;
using Confluent.Kafka;
using EasyDdd.Billing.Core;
using EasyDdd.Billing.Data;
using EasyDdd.Billing.Web.Converters;
using EasyDdd.Billing.Web.Messaging;
using EasyDdd.Kernel;
using EasyDdd.Kernel.Kafka;
using EasyDdd.ShipmentManagement.Core;
using MediatR;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.EntityFrameworkCore;
using IClock = NodaTime.IClock;
using Shipment = EasyDdd.Billing.Core.Shipment;
using SystemClock = NodaTime.SystemClock;

var builder = WebApplication.CreateBuilder(args);

var kafkaConfiguration = builder.Configuration.GetSection("Kafka");

builder.Services.AddMediatR(typeof(BillingContext), typeof(Shipment), typeof(MessageHub));
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
builder.Services.AddSignalR(options =>
	{
		options.KeepAliveInterval = TimeSpan.FromSeconds(5);
	})
	.AddJsonProtocol(options =>
	{
		options.PayloadSerializerOptions = new JsonSerializerOptions().ConfigureConverters();
		options.PayloadSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
	});
builder.Services.Configure<BillingOptions>(builder.Configuration.GetSection(BillingOptions.Billing));

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

builder.Services.AddKafkaConsumer(options =>
{
    options.TopicNames.Add("shipmentmanagement-shipments");
    options.TopicNames.Add("billing-statements");
    options.ConsumerConfig.BootstrapServers = kafkaConfiguration["Endpoint"];
    options.ConsumerConfig.GroupId = "billing";
    options.ConsumerConfig.AllowAutoCreateTopics = builder.Environment.IsDevelopment();
    options.ConsumerConfig.AutoOffsetReset = AutoOffsetReset.Latest;

    if (builder.Environment.IsDevelopment())
    {
        options.ConsumerConfig.SecurityProtocol = SecurityProtocol.Plaintext;
        return;
    }

    // If you're using Confluent Cloud, for example.
    options.ConsumerConfig.SecurityProtocol = SecurityProtocol.SaslSsl;
    options.ConsumerConfig.SaslMechanism = SaslMechanism.Plain;
    options.ConsumerConfig.SaslUsername = kafkaConfiguration["ApiKey"];     // Set secret during deployment
    options.ConsumerConfig.SaslPassword = kafkaConfiguration["ApiSecret"];  // Set secret during deployment
}).AddMediatRMessageHandler(options =>
{
    options.JsonSerializerOptions.ConfigureConverters();
    options.NotificationTypes.Add(typeof(ShipmentCreated));
    options.NotificationTypes.Add(typeof(ShipmentDelivered));
    options.NotificationTypes.Add(typeof(ShipmentDispatched));
    options.NotificationTypes.Add(typeof(ShipmentRated));
    options.NotificationTypes.Add(typeof(ShipmentStatusUpdated));
    options.NotificationTypes.Add(typeof(TrackingEventAdded));
	options.NotificationTypes.Add(typeof(StatementCreated));
	options.NotificationTypes.Add(typeof(StatementLineAdded));
}).AddDeadLetterExceptionHandler();

TypeDescriptor.AddAttributes(typeof(StatementIdentifier), new TypeConverterAttribute(typeof(StatementIdentifierConverter)));

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

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();
app.MapHub<MessageHub>("/messageHub", options =>
{
	options.Transports = HttpTransportType.WebSockets;
	
});
app.Run();
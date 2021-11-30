﻿using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Reflection;
using AzureEventGridSimulator.Domain.Services;
using AzureEventGridSimulator.Infrastructure.Middleware;
using AzureEventGridSimulator.Infrastructure.Settings;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AzureEventGridSimulator
{
    public class Startup
    {
        private readonly ILoggerFactory _loggerFactory;

        public Startup(IConfiguration configuration, ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
            Configuration = configuration;
        }

        private IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            var settings = new SimulatorSettings();
            Configuration.Bind(settings);
            settings.Validate();
            services.AddSingleton(o => settings);

            services.AddMediatR(Assembly.GetExecutingAssembly());
            services.AddHttpClient("default")
                .ConfigurePrimaryHttpMessageHandler(() =>
                {
                    // When the simulator is running inside docker, but talking to services running
                    // on the host machine using self signed certs, you get connection errors because
                    // the self signed certs are not trusted by the container OS.  This lets us ignore
                    // those certificate errors.
					var handler = new HttpClientHandler
					{
						ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
					};
					return handler;
                });

            services.AddHostedService<SubscriptionValidationService>();
            services.AddSingleton(o => _loggerFactory.CreateLogger(nameof(AzureEventGridSimulator)));
            services.AddScoped<SasKeyValidator>();
            services.AddSingleton<ValidationIpAddress>();

            services.AddMvc();
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseMiddleware<EventGridMiddleware>();
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }

    public class ValidationIpAddress
    {
        private readonly string _ipAddress;

        public ValidationIpAddress()
        {
            var hostName = Dns.GetHostName();
            _ipAddress = Dns.GetHostAddresses(hostName).First(ip => ip.AddressFamily == AddressFamily.InterNetwork &&
                                                                    !IPAddress.IsLoopback(ip)).ToString();
        }

        public override string ToString()
        {
            return _ipAddress;
        }

        public static implicit operator string(ValidationIpAddress d) => d.ToString();
    }
}

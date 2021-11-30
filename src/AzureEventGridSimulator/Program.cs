﻿using System;
using System.Net;
using AzureEventGridSimulator.Infrastructure.Settings;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace AzureEventGridSimulator
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                var host = WebHost
                    .CreateDefaultBuilder(args)
                    .ConfigureAppConfiguration((context, builder) =>
                    {
                        var env = context.HostingEnvironment;

                        var config = builder.Build();

                        var configFile = config.GetValue<string>("ConfigFile");
                        if (!string.IsNullOrEmpty(configFile))
                        {
                            builder.AddJsonFile(configFile, optional: false);
                        }

                        Log.Logger = new LoggerConfiguration()
                            .Enrich.FromLogContext()
                            .Enrich.WithProperty("AspNetCoreEnvironment", env.EnvironmentName)
                            .Enrich.WithMachineName()
                            .MinimumLevel.Debug()
                            .MinimumLevel.Override("Microsoft", LogEventLevel.Error)
                            .MinimumLevel.Override("System", LogEventLevel.Error)
                            .ReadFrom.Configuration(config)
                            .CreateLogger();
                    })
                    .UseStartup<Startup>()
                    .UseSerilog()
                    .UseKestrel(options =>
                    {
                        var simulatorSettings = (SimulatorSettings)options.ApplicationServices.GetService(typeof(SimulatorSettings));
                        var certPath = Environment.GetEnvironmentVariable("ASPNETCORE_Kestrel__Certificates__Default__Path");
                        var certPassword = Environment.GetEnvironmentVariable("ASPNETCORE_Kestrel__Certificates__Default__Password");

                        foreach (var topics in simulatorSettings.Topics)
                        {
                            options.Listen(
                                IPAddress.Any,
                                topics.Port,
                                listenOptions =>
                                {
                                    listenOptions.UseHttps(certPath, certPassword);
                                }
                            );
                        }
                    })
                    .Build();

                var logger = (ILogger)host.Services.GetService(typeof(ILogger));
                logger.LogInformation("Started");

                try
                {
                    host.Run();
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Failed to run the Azure Event Grid Simulator.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            finally
            {
                Log.CloseAndFlush();
            }

            Console.WriteLine("");
            Console.WriteLine("Any key to exit...");
            Console.ReadKey();
        }
    }
}

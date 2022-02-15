using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Azure;
using Azure.Core.Serialization;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Azure.Messaging.EventGrid;
using Azure.Messaging.EventGrid.Models;
using Azure.Messaging.EventGrid.SystemEvents;
using Microsoft.Extensions.DependencyInjection;

namespace EasyDdd.Kernel.EventGrid
{
    public class EventGridMiddleware
    {
        private readonly RequestDelegate _next;
		private readonly IServiceScopeFactory _serviceScopeFactory;
		private readonly EventGridConfiguration _config;

        public EventGridMiddleware(RequestDelegate next, IServiceScopeFactory serviceScopeFactory, EventGridConfiguration config)
        {
            _next = next;
			_serviceScopeFactory = serviceScopeFactory;
			_config = config;
        }

        public async Task Invoke(HttpContext context, ILogger<EventGridMiddleware> logger)
		{
			if (!context.Request.Query.TryGetValue("key", out var value) || value.Count != 1 || value.Single() != _config.ApiKey)
            {
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                return;
            }

            var requestContent = await ReadBody(context.Request);

            var eventGridEvents = EventGridEvent.ParseMany(BinaryData.FromString(requestContent));

			if (eventGridEvents.Length == 0)
            {
                logger.LogWarning("Received empty request to EventGrid endpoint");
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return;
            }

			if (eventGridEvents.Length == 1 && eventGridEvents[0].TryGetSystemEventData(out object systemEvent) && systemEvent is SubscriptionValidationEventData validationEventData)
			{
				logger.LogInformation($"Subscribed to EventGrid with code {validationEventData.ValidationCode}");

				var response = new SubscriptionValidationResponse
				{
					ValidationResponse = validationEventData.ValidationCode
				};

                var responseContent = JsonSerializer.Serialize(response);

                context.Response.StatusCode = (int)HttpStatusCode.OK;
                await context.Response.WriteAsync(responseContent);
                return;
            }

            var domainEvents = new List<(string key, object value)>();
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var eventGridEvent in eventGridEvents)
            {
                try
                {
                    logger.LogInformation($"Received event with data of type {eventGridEvent.EventType}");

                    var eventDataType = assemblies
                        .Select(assembly => assembly.GetType(eventGridEvent.EventType, false, true))
                        .FirstOrDefault(type => !(type is null));

                    if (eventDataType is null)
                    {
                        logger.LogError($"Event ${eventGridEvent.Id} not processed.  No type with name {eventGridEvent.EventType} exists.");
                        continue;
                    }

					var json = eventGridEvent.Data.ToString().Replace("\\u0022", "\"");
                    json = json[json.IndexOf("{", StringComparison.InvariantCulture)..];
					json = json[..(json.LastIndexOf("}", StringComparison.InvariantCulture) + 1)];
                    
					var domainEvent = JsonSerializer.Deserialize(json, eventDataType, _config.JsonOptions);
                    if (domainEvent is null)
                    {
                        logger.LogError($"Event ${eventGridEvent.Id} not processed.  Data deserialized to null.");
                        continue;
                    }

                    domainEvents.Add((eventGridEvent.Id, domainEvent));
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, $"An error occurred deserializing event ${eventGridEvent.Id}.");
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    return;
                }
            }

			using var scope = _serviceScopeFactory.CreateScope();

			var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

            foreach (var domainEvent in domainEvents)
            {
                try
                {
                    await mediator.Publish(domainEvent.value);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, $"An error occurred publishing event ${domainEvent.key}.");
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    return;
                }
            }

            context.Response.StatusCode = (int)HttpStatusCode.NoContent;
        }

        private async Task<string> ReadBody(HttpRequest request)
		{
			using var reader = new StreamReader(request.Body, Encoding.UTF8);

			return await reader.ReadToEndAsync();
		}
    }
}

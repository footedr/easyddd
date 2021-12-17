using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.EventGrid;
using Microsoft.Azure.EventGrid.Models;
using Microsoft.Extensions.Logging;

namespace EasyDdd.Kernel.EventGrid
{
    public class EventGridMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly EventGridConfiguration _config;

        public EventGridMiddleware(RequestDelegate next, EventGridConfiguration config)
        {
            _next = next;
            _config = config;
        }

        public async Task Invoke(HttpContext context, IMediator mediator, ILogger<EventGridMiddleware> logger)
		{
			if (!context.Request.Query.TryGetValue("key", out var value) || value.Count != 1 || value.Single() != _config.ApiKey)
            {
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                return;
            }

            var requestContent = await ReadBody(context.Request);
            var eventGridEvents = new EventGridSubscriber().DeserializeEventGridEvents(requestContent);
            if (eventGridEvents.Length == 0)
            {
                logger.LogWarning("Received empty request to EventGrid endpoint");
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return;
            }

            if (eventGridEvents.Length == 1 && eventGridEvents[0].Data is SubscriptionValidationEventData validationEventData)
            {
                logger.LogInformation($"Subscribed to EventGrid with code {validationEventData.ValidationCode}");

                var response = new SubscriptionValidationResponse(validationEventData.ValidationCode);
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
                    logger.LogInformation($"Received event with data of type {eventGridEvent.Data.GetType().FullName}");

                    var eventDataType = assemblies
                        .Select(assembly => assembly.GetType(eventGridEvent.EventType, false, true))
                        .FirstOrDefault(type => !(type is null));

                    if (eventDataType is null)
                    {
                        logger.LogError($"Event ${eventGridEvent.Id} not processed.  No type with name {eventGridEvent.EventType} exists.");
                        continue;
                    }

                    var json = eventGridEvent.Data.ToString();
                    if (json is null)
                    {
                        logger.LogError($"Event ${eventGridEvent.Id} not processed.  Empty data.");
                        continue;
                    }

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
            return;
        }

        private async Task<string> ReadBody(HttpRequest request)
        {
            using (var reader = new StreamReader(request.Body, Encoding.UTF8))
            {
                return await reader.ReadToEndAsync();
            }
        }
    }
}

using System;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AzureEventGridSimulator.Domain.Entities;
using AzureEventGridSimulator.Infrastructure.Extensions;
using AzureEventGridSimulator.Infrastructure.Settings;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace AzureEventGridSimulator.Domain.Commands
{
    public class SendNotificationEventsToSubscriberCommandHandler : AsyncRequestHandler<SendNotificationEventsToSubscriberCommand>
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger _logger;

        public SendNotificationEventsToSubscriberCommandHandler(IHttpClientFactory httpClientFactory, ILogger logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        protected override Task Handle(SendNotificationEventsToSubscriberCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("{EventCount} event(s) received on topic '{TopicName}'", request.Events.Length, request.Topic.Name);

            foreach (var eventGridEvent in request.Events)
            {
                eventGridEvent.Topic = $"/subscriptions/{Guid.Empty:D}/resourceGroups/eventGridSimulator/providers/Microsoft.EventGrid/topics/{request.Topic.Name}";
                eventGridEvent.MetadataVersion = "1";
            }

            foreach (var subscription in request.Topic.Subscribers)
            {
#pragma warning disable 4014
                SendToSubscriber(subscription, request.Events);
#pragma warning restore 4014
            }

            return Task.CompletedTask;
        }

        private async Task SendToSubscriber(SubscriptionSettings subscription, EventGridEvent[] events)
        {
            var attempts = 0;

            while (true)
            {
                try
                {
                    if (!subscription.DisableValidation &&
                        subscription.ValidationStatus != SubscriptionValidationStatus.ValidationSuccessful)
                    {
                        _logger.LogWarning("Subscription '{SubscriberName}' can't receive events. It's still pending validation.", subscription.Name);
                        return;
                    }

                    _logger.LogDebug("Sending to subscriber '{SubscriberName}'.", subscription.Name);

                    // "Event Grid sends the events to subscribers in an array that has a single event. This behaviour may change in the future."
                    // https://docs.microsoft.com/en-us/azure/event-grid/event-schema
                    foreach (var evt in events)
                    {
                        if (subscription.Filter.AcceptsEvent(evt))
                        {
                            var json = JsonConvert.SerializeObject(new[] { evt }, Formatting.Indented);
                            using (var content = new StringContent(json, Encoding.UTF8, "application/json"))
                            {
                                var httpClient = _httpClientFactory.CreateClient("default");
                                httpClient.DefaultRequestHeaders.Add("aeg-event-type", "Notification");
                                httpClient.Timeout = TimeSpan.FromSeconds(120);

                                var response = await httpClient.PostAsync(subscription.Endpoint, content);
                                response.EnsureSuccessStatusCode();
                                _logger.LogDebug("Event {EventId} sent to subscriber '{SubscriberName}' successfully.", evt.Id, subscription.Name);
                            }
                        }
                        else
                        {
                            _logger.LogWarning("Event {EventId} {EventType} filtered out for subscriber '{SubscriberName}'.", evt.Id, evt.EventType, subscription.Name);
                        }
                    }
                }
                catch (Exception ex)
                {
                    attempts++;
                    _logger.LogError(ex, $"Failed to send to subscriber '{subscription.Name}', attempt {attempts}.");

                    if (attempts > 3)
                    {
                        return;
                    }
                    else
                    {
                        await Task.Delay(1000);
                        continue;
                    }
                }

                return;
            }
        }

        private void LogResult(Task<HttpResponseMessage> task, EventGridEvent evt, SubscriptionSettings subscription)
        {
            if (task.IsCompletedSuccessfully && task.Result.IsSuccessStatusCode)
            {
                _logger.LogDebug("Event {EventId} sent to subscriber '{SubscriberName}' successfully.", evt.Id, subscription.Name);
            }
            else
            {
                _logger.LogError(task.Exception?.GetBaseException(),
                                 "Failed to send event {EventId} to subscriber '{SubscriberName}', '{TaskStatus}', '{Reason}'.",
                                 evt.Id,
                                 subscription.Name,
                                 task.Status.ToString(),
                                 task.Result?.ReasonPhrase);
            }
        }
    }
}

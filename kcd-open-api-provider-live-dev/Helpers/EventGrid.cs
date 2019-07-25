using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.EventGrid;
using Microsoft.Azure.EventGrid.Models;

namespace OpenApiProvider.Helpers
{
    internal class EventGrid
    {
        private static string _topicHostname;
        private static EventGridClient _client;

        internal static void SetupEventGrid(string topicEndpoint, string topicKey)
        {
            var topicCredentials = new TopicCredentials(topicKey);

            _topicHostname = new Uri(topicEndpoint).Host;
            _client = new EventGridClient(topicCredentials);
        }

        internal static async Task SendReferenceEvent(string apiReference, string eventType, string isTest = "disabled")
        {
            var events = new List<EventGridEvent> {CreateEventGridEvent(apiReference, eventType, isTest) };

            await _client.PublishEventsAsync(_topicHostname, events);
        }

        private static EventGridEvent CreateEventGridEvent(string apiReference, string eventType, string isTest)
            => new EventGridEvent
            {
                Id = Guid.NewGuid().ToString(),
                EventType = eventType,
                Data = new
                {
                    apiReference,
                    isTest
                },
                EventTime = DateTime.Now,
                DataVersion = "1.0"
            };
    }
}
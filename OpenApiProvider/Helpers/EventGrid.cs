using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.EventGrid;
using Microsoft.Azure.EventGrid.Models;
using OpenApiProvider.Models;

namespace OpenApiProvider.Helpers
{
    internal class EventGrid
    {
        private readonly string _topicHostname;
        private readonly EventGridClient _client;

        internal EventGrid(string topicEndpoint, string topicKey)
        {
            var topicCredentials = new TopicCredentials(topicKey);

            _topicHostname = new Uri(topicEndpoint).Host;
            _client = new EventGridClient(topicCredentials);
        }

        internal async Task SendReferenceEvent(string apiReference, string eventType, string isTest = "disabled")
        {
            var events = new List<EventGridEvent>
            {
                CreateEventGridEvent(new EventGridData
                    {
                        ApiReference = apiReference,
                        IsTest = isTest
                    },
                    eventType
                )
            };

            await _client.PublishEventsAsync(_topicHostname, events);
        }

        private EventGridEvent CreateEventGridEvent(EventGridData eventGridData, string eventType)
            => new EventGridEvent
            {
                Id = Guid.NewGuid().ToString(),
                Subject = "event",
                EventType = eventType,
                Data = eventGridData,
                EventTime = DateTime.Now,
                DataVersion = "1.0"
            };
    }
}
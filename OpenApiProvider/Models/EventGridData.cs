using System;
using Newtonsoft.Json;

namespace OpenApiProvider.Models
{
    [Serializable]
    public class EventGridData
    {
        [JsonProperty(PropertyName = "apiReference")]
        public string ApiReference { get; set; }

        [JsonProperty(PropertyName = "isTest")]
        public string IsTest { get; set; }
    }
}

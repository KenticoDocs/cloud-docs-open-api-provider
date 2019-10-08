using System;

namespace OpenApiProvider.Models
{
    [Serializable]
    public class OrchestratorInput
    {
        public string IsPreview { get; set; }
        public string IsTest { get; set; }
    }
}

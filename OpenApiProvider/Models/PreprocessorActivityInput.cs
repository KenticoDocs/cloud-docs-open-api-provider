using System;

namespace OpenApiProvider.Models
{
    [Serializable]
    public class PreprocessorActivityInput : OrchestratorInput
    {
        public string Codename { get; set; }
    }
}

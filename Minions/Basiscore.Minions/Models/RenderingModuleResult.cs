
namespace Basiscore.Minions.Models
{
    using System.Collections.Generic;

    public class RenderingModuleResult
    {
        public int StatusCode { get; set; }

        public string StatusMessage { get; set; }

        public string Error { get; set; }

        public List<RenderingTaskStatus> LstRenderingStatus { get; set; }
    }
}
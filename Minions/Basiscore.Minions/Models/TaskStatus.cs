
namespace Basiscore.Minions.Models
{
    public class TaskStatus
    {
        public int StatusCode { get; set; }

        public string StatusMessage { get; set; }

        public string FileName { get; set; }

        public string InvalidPaths { get; set; }
    }
}
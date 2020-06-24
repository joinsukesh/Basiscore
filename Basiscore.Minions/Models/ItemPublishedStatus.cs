
namespace Basiscore.Minions.Models
{
    public class ItemPublishedStatus
    {
        public string ItemPath { get; set; }
        public string ItemsCreatedStatus { get; set; }
        public string ItemsUpdatedStatus { get; set; }
        public string ItemsSkippedStatus { get; set; }
        public int ItemsCreated { get; set; }
        public int ItemsUpdated { get; set; }
        public int ItemsSkipped { get; set; }
        public string TargetDatabase { get; set; }
        public string Error { get; set; }        
        public bool CautionUser { get; set; }        
    }
}
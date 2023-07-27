namespace Basiscore.CmsAudit.Models
{
    using System;

    public class AuditLog_Item
    {
        public int RowId { get; set; }
        public Guid ItemId { get; set; }
        public string ItemName { get; set; }
        public string ItemPath { get; set; }
        public Guid TemplateId { get; set; }
        public string ItemLanguage { get; set; }
        public int ItemVersion { get; set; }
        public string Event { get; set; }
        public string ActionedBy { get; set; }
        public string ItemDataBeforeSave { get; set; }
        public string ItemDataAfterSave { get; set; }
        public string Comments { get; set; }
        public DateTime LoggedTime { get; set; }
    }    
}
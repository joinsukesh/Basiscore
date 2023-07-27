namespace Basiscore.CmsAudit.Models
{
    using System;
    using System.Collections.Generic;

    public class ItemInfo
    {
        public Guid ItemId { get; set; }
        public string ItemName { get; set; }        
        public string ItemPath { get; set; }
        public Guid TemplateId { get; set; }
        public string ItemLanguage { get; set; }
        public int ItemVersion { get; set; }        
        public List<KeyValuePair<string, string>> FieldData { get; set; }
    }
}
using System;

namespace Basiscore.CmsAudit.Models
{
    public class ItemAuditLogRequest
    {
        public int LogType { get; set; }
        public bool GetOnlySitePublishLogs
        {
            get
            {
                return LogType == 3;
            }
        }

        public bool GetOnlyItemPublishLogs
        {
            get
            {
                return LogType == 2;
            }
        }

        public bool GetOnlyPublishLogs
        {
            get
            {
                return LogType == 4;
            }
        }

        public string ItemId { get; set; }
        public string ActionedBy { get; set; }
        public string ItemLanguage { get; set; }
        public string FromDateString { get; set; }

        public DateTime FromDate
        {
            get
            {
                return Convert.ToDateTime(FromDateString);
            }
        }

        public string ToDateString { get; set; }

        public DateTime ToDate
        {
            get
            {
                return Convert.ToDateTime(ToDateString);
            }
        }
    }
}
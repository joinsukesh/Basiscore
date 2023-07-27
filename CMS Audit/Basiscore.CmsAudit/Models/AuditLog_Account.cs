namespace Basiscore.CmsAudit.Models
{
    using System;

    public class AuditLog_Account
    {
        public Guid UserId { get; set; }
        public string Username { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Domain { get; set; }
        public bool IsAdministrator { get; set; }
        public string Roles { get; set; }
        public string Event { get; set; }
        public string ActionedBy { get; set; }
        public string Comments { get; set; }
        public DateTime LoggedTime { get; set; }
    }
}
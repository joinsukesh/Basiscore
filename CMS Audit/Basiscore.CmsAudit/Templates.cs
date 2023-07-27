namespace Basiscore.CmsAudit
{
    using Sitecore.Data;

    public class Templates
    {
        public struct System
        {
            public struct Tasks
            {
                /// <summary>
                /// /sitecore/templates/System/Tasks/Schedule
                /// </summary>
                public static ID Schedule => new ID("{70244923-FA84-477C-8CBD-62F39642C42B}");
            }
        }

        public struct DictionaryEntry
        {
            public static ID ID => new ID("{6D1CD897-1936-4A3A-A511-289A94C2A7B1}");

            public struct Fields
            {
                public static ID Phrase => new ID("{2BA3454A-9A9C-4CDF-A9F8-107FD484EB6E}");

                public static ID Key => new ID("{580C75A8-C01A-4580-83CB-987776CEB3AF}");
            }
        }
    }
}
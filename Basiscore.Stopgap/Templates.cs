using Sitecore.Data;

namespace Basiscore.Stopgap
{
    public class Templates
    {
        public struct StopgapBase
        {
            public static readonly ID ID = new ID("{33DAF426-E5B4-40A1-8ADD-6DCD8878AD08}");

            public struct Fields
            {
                public static readonly ID IsActive = new ID("{5374B1BA-1A05-4E12-ABEB-0228E8231E0C}");
            }
        }

        public struct StopgapPage
        {
            public static readonly ID ID = new ID("{D0ABC2A6-9E1C-42C7-AE80-EC686F4ED590}");
        }

        public struct StopgapDatasource
        {
            public static readonly ID ID = new ID("{C28ED370-597C-4DCD-9DE3-706B12130B87}");

            public struct Fields
            {
                public static readonly ID HTML = new ID("{0071FF99-20CA-43D0-88B4-3228C54D01A3}");
            }
        }

        public struct RichText
        {
            public static readonly ID ID = new ID("{AFDA530E-DECE-4C04-BC98-90191EB8FAE7}");

            public struct Fields
            {
                public static readonly ID Content = new ID("{92B378E5-A6F4-47EA-A168-0524F9E8C86A}");
            }
        }


    }
}
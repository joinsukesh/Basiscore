
namespace Basiscore.ContentAuthoringGuide
{
    using Sitecore.Data;

    public struct Templates
    {
        public struct GuideRoot
        {
            public static readonly ID ID = new ID("{E1D35ECB-79E9-41A3-B47C-4BB6813E0298}");

            public struct Fields
            {
                public static readonly ID TopLogo = new ID("{696A1F17-5D61-423F-A3A4-899874DFE30E}");
                public static readonly ID TopLogoContent = new ID("{0698D819-21E4-4E4A-800D-FF5572EE6142}");
                public static readonly ID Title = new ID("{A9BD8191-522D-4AB9-BBE2-500D09A5FDF7}");
                public static readonly ID CoverPageImage = new ID("{1D395BF0-6CE5-44C1-A024-5FE3FA69DBBF}");
                public static readonly ID BottomLogo = new ID("{DB80B961-D327-42F4-8A77-65C982938220}");
                public static readonly ID BottomLogoContent = new ID("{C20E2DA8-00FC-4973-B6A6-DD2C7891E6E6}");
            }
        }

        public struct Article
        {
            public static readonly ID ID = new ID("{5E55D8E0-F5D0-4EA9-B30D-F18D4E2FEE36}");

            public struct Fields
            {
                public static readonly ID ReferenceURL = new ID("{D9473784-D3AB-491E-A350-9C03074690F0}");
                public static readonly ID Slides = new ID("{DE3030F1-1DAE-4A57-89F6-6321C7750AED}");
                public static readonly ID Section1Header = new ID("{24A5323F-5572-46E7-931A-7959E47EF06F}");
                public static readonly ID Section1Description = new ID("{545D262F-C6C9-4D51-87E0-93F3101F5810}");
                public static readonly ID Section2Header = new ID("{6D9440C6-FBF4-43D6-9408-3934EA627EBB}");
                public static readonly ID Section2Description = new ID("{97A97C8D-2C7D-4C5B-84F1-0C3A2104ABAA}");
                public static readonly ID Section3Header = new ID("{FC84AE5B-D45D-4F59-9640-C1EEF607E3C7}");
                public static readonly ID Section3Description = new ID("{AE560313-B582-40DC-A51D-B5DA53B28A7A}");
                public static readonly ID Section4Header = new ID("{8C06CE90-90A5-4799-B56F-410AE36073DD}");
                public static readonly ID Section4Description = new ID("{021104FF-CC37-49AA-8F21-E6BA5A765AF6}");
                public static readonly ID Section5Header = new ID("{E32A99DC-C947-4AAD-A38A-8C6E1D7041D7}");
                public static readonly ID Section5Description = new ID("{51F9912E-443F-45A0-8195-10CB9AF544EF}");
                public static readonly ID IsActive = new ID("{45EE9EB0-2EF8-4E53-816F-AD172FD22CD0}");
                public static readonly ID Title = new ID("{19FF5E7F-BF86-468B-A21D-06209B2506C8}");
                public static readonly ID Description = new ID("{B55A427F-EE8F-4339-BD2A-A8ED68847745}");
            }
        }

        public struct ArticlesFolder
        {
            public static readonly ID ID = new ID("{08B2108E-1721-4834-8FC8-6E3D062441AC}");

            public struct Fields
            {
                public static readonly ID Title = new ID("{4BB83624-95E3-4AE7-9DA0-C28FAFD84518}");
                public static readonly ID IsActive = new ID("{37CFD818-4569-4FF5-8F2A-D019AFED87BD}");
            }
        }

        public struct CarouselSlide
        {
            public static readonly ID ID = new ID("{44E4BF0E-317C-49A1-919F-37ACCABE32D7}");

            public struct Fields
            {
                public static readonly ID Image = new ID("{3E06F337-763F-4199-B758-942993B9C3AF}");
                public static readonly ID Description = new ID("{2B059E8C-EDED-4273-AABD-03A74F729A02}");
                public static readonly ID IsActive = new ID("{42228296-49C6-4BCC-B9CC-844FDD202329}");
            }
        }
    }
}
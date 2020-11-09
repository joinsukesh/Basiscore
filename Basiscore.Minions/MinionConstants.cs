
namespace Basiscore.Minions
{
    using Sitecore.Data;
    using System.Collections.Generic;

    public class MinionConstants
    {
        public static string ValidCharacters = "abcdefghijklmnopqrstuvwxyz0123456789.-_ ";
        public static string Timestamp = "Timestamp";
        public static string One = "1";
        
        public struct DatabaseNames
        {
            //public static string Core = "core";
            public static string Master = "master";
            //public static string Web = "web";
        }

        public struct ErrorMessages
        {
            public static string General = "A problem occurred while processing your request.";
        }

        public List<object> MyProperty { get; set; }

        public struct FieldTypes
        {
            public const string Checkbox = "Checkbox";
            public const string Image = "Image";
            public const string GeneralLink = "General Link";
            public const string SingleLineText = "Single-Line Text";
            public const string MultiLineText = "Multi-Line Text";
            public const string RichText = "Rich Text";
            public const string Multilist = "Multilist";
            public const string MultilistWithSearch = "Multilist with Search";
            public const string Treelist = "Treelist";
            public const string TreelistEx = "TreelistEx";
            public const string Integer = "Integer";
            public const string Number = "Number";
            public const string Date = "Date";
            public const string Datetime = "Datetime";
            public const string Droplink = "Droplink";
            public const string Droptree = "Droptree";
        }

        public struct FieldNames
        {
            public static string FinalRenderings = "__Final Renderings";
        }

        public struct Paths
        {
            public static string LoginPagePath = "~/sitecore/login";
            public static string Templates = "/sitecore/templates/";
            public static string SystemTemplates = "/sitecore/templates/system/";
            public static string BranchTemplate = "/sitecore/templates/branches/";
            public static string Content = "/sitecore/content/";
        }

        public struct Items
        {
            public static ID PublishingTargets = new ID("{D9E44555-02A6-407A-B4FC-96B9026CAADD}");

            /// /sitecore/layout/Devices/Default
            public static string DefaultLayoutDeviceId = "{FE5D7FDF-89C0-4D99-9AA3-B5FBD009C9F3}";
        }

        public struct Templates
        {
            /// <summary>
            /// /sitecore/templates/System/Publishing target
            /// </summary>
            public struct PublishingTarget
            {
                public static ID ID = new ID("{E130C748-C13B-40D5-B6C6-4B150DC3FAB3}");

                public struct Fields
                {
                    public static ID TargetDatabase = new ID("{39ECFD90-55D2-49D8-B513-99D15573DE41}");
                }
            }

            /// <summary>
            /// /sitecore/templates/System/Templates/Sections/Workflow
            /// </summary>
            public struct Workflow
            {
                public static ID ID = new ID("{06F366E6-A7E6-470B-9EC9-CD29A4F6C8E8}");

                public struct Fields
                {
                    public static ID Workflow = new ID("{A4F985D9-98B3-4B52-AAAF-4344F6E747C6}");
                    public static ID WorkflowState = new ID("{3E431DE1-525E-47A3-B6B0-1CCBEC3A8C98}");
                    public static ID Lock = new ID("{001DD393-96C5-490B-924A-B0F25CD9EFD8}");
                    public static ID DefaultWorkflow = new ID("{CA9B9F52-4FB0-4F87-A79F-24DEA62CDA65}");
                }
            }

            /// <summary>
            /// /sitecore/templates/System/Templates/Template
            /// </summary>
            public struct Template
            {
                public static ID ID = new ID("{AB86861A-6030-46C5-B394-E8F99E8B87DB}");
            }

            /// <summary>
            /// /sitecore/templates/System/Templates/Template section
            /// </summary>
            public struct TemplateSection
            {
                public static ID ID = new ID("{E269FBB5-3750-427A-9149-7AA950B49301}");
            }

            /// <summary>
            /// /sitecore/templates/System/Templates/Template field
            /// </summary>
            public struct TemplateField
            {
                public static ID ID = new ID("{455A3E98-A627-4B40-8035-E683A0331AC7}");
            }

            /// <summary>
            /// /sitecore/templates/System/Templates/Standard template
            /// </summary>
            public struct StandardTemplate
            {
                public static ID ID = new ID("{1930BBEB-7805-471A-A3BE-4858AC7CF696}");
            }

            public struct Publishing
            {
                public static ID ID = new ID("{6495CF23-DE9C-48B7-9D3C-05E2418B3CAE}");

                public struct Fields
                {
                    public static ID NeverPublish = new ID("{9135200A-5626-4DD8-AB9D-D665B8C11748}");
                }
            }

            public struct Layout
            {
                public static ID ID = new ID("{4D30906D-0B49-4FA7-969D-BF90157357EA}");

                public struct Fields
                {
                    public static ID __Renderings = new ID("{F1A1FE9E-A60C-4DDB-A3A0-BB5B29FE732E}");
                    public static ID __FinalRenderings = new ID("{04BF00DB-F5FB-41F7-8AB7-22408372A981}");
                }
            }
        }
    }
}


namespace Basiscore.Minions.Models
{
    using Basiscore.Minions.Utilities;
    using Sitecore;
    using Sitecore.Globalization;
    using System.Collections.Generic;
    using System.Linq;

    public class RenderingsModuleDataModel
    {
        public int TaskId { get; set; }
        public string ParentItemId { get; set; }
        public string TargetTemplateId { get; set; }
        public string RenderingId { get; set; }
        public string Placeholder { get; set; }
        public string DatasourceId { get; set; }
        public string InputRenderingIndex { get; set; }
        public int TargetItemsTypeId { get; set; }

        public int RenderingIndex
        {
            get
            {
                if (string.IsNullOrEmpty(this.InputRenderingIndex))
                {
                    return -1;
                }
                else
                {
                    return MainUtil.GetInt(this.InputRenderingIndex, -1);
                }
            }
        }

        public bool CreateVersion { get; set; }
        
        public string TargetItemPaths { get; set; }

        public List<string> GetItemPaths()
        {
            List<string> lstItemPaths = new List<string>();
            string[] arrItemPaths = TargetItemPaths != null ? TargetItemPaths.Split('\n') : null;
            string validItemPath = "";

            if (arrItemPaths != null && arrItemPaths.Length > 0)
            {
                ///for each string remove trailing / and add only unique paths to the list.
                foreach (string itemPath in arrItemPaths)
                {
                    validItemPath = itemPath.Trim().TrimEnd('/');

                    if (!string.IsNullOrEmpty(validItemPath)
                        && !lstItemPaths.Any(x => x == itemPath))
                    {
                        lstItemPaths.Add(validItemPath);
                    }
                }
            }

            return lstItemPaths;
        }

        public string TargetLanguageCode { get; set; }

        public Language SelectedLanguage
        {
            get
            {
                return MinionHelper.GetTargetLanguages(TargetLanguageCode).FirstOrDefault();
            }
        }

        public int TargetLayoutId { get; set; }

        public bool CopyFinalRenderingsToShared { get; set; }
    }
}
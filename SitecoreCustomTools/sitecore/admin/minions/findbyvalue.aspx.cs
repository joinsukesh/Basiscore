
namespace SitecoreCustomTools.sitecore.admin.minions
{
    using Sitecore.Data;
    using Sitecore.Data.Fields;
    using Sitecore.Data.Items;
    using Sitecore.Globalization;
    using SitecoreCustomTools.Models;
    using SitecoreCustomTools.Utilities;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Web.Script.Serialization;
    using System.Web.Services;

    public partial class findbyvalue : System.Web.UI.Page
    {
        #region EVENTS

        protected void Page_Load(object sender, EventArgs e)
        {
            if (SctHelper.IsUserLoggedIn())
            {
                try
                {
                    BindData();
                }
                catch (Exception ex)
                {

                }
            }
            else
            {
                Response.Redirect(SctConstants.Paths.LoginPagePath);
            }
        }

        [WebMethod]
        public static string FindItems(FindByValueDataModel findByValueDataModel)
        {
            FindByValueReport result = new FindByValueReport();
            string error = "";
            string output = "";

            try
            {
                if (SctHelper.IsUserLoggedIn())
                {
                    if (IsValidModel(findByValueDataModel))
                    {
                        List<ValueMatchedItem> lstValueMatchedItems = FindItemsByValue(findByValueDataModel, out error);

                        if (string.IsNullOrEmpty(error))
                        {
                            if (lstValueMatchedItems != null && lstValueMatchedItems.Count > 0)
                            {
                                result.TaskStatus = 1;
                                result.LstValueMatchedItems = GetMatchLog(lstValueMatchedItems, findByValueDataModel.SelectedLanguages);
                            }
                            else
                            {
                                result.TaskStatus = 0;
                                result.Error = "No matches found for the keyword";
                            }                            
                        }
                        else
                        {
                            result.TaskStatus = 0;
                            result.Error = error;
                        }
                    }
                    else
                    {
                        result.Error = "Invalid input";
                    }
                }
                else
                {
                    result.TaskStatus = 2;
                }
            }
            catch (Exception ex)
            {
                result.LstValueMatchedItems = new List<ValueMatchedItem>();
                result.TaskStatus = 0;
                result.Error = ex.Message;
            }

            output = new JavaScriptSerializer().Serialize(result);
            return output;
        }

        #endregion

        #region METHODS

        private void BindData()
        {
            chkLanguages.DataSource = SctHelper.GetInstalledLanguages();
            chkLanguages.DataTextField = "Value";
            chkLanguages.DataValueField = "Key";
            chkLanguages.DataBind();
        }

        /// <summary>
        /// For the model to be valid, it should:
        /// 1. have the parent item id
        /// 2. have the keyword
        /// 3. have atleast one target language 
        /// </summary>
        /// <param name="customPublishDataModel"></param>
        /// <returns></returns>
        private static bool IsValidModel(FindByValueDataModel dataModel)
        {
            bool isValidModel = false;
            if (dataModel != null && dataModel.TaskId > 0 && dataModel.TaskId < 3 &&
                !string.IsNullOrEmpty(dataModel.ParentItemId) && !string.IsNullOrEmpty(dataModel.Keyword) &&
                dataModel.MatchCondition > 0 && dataModel.TaskId < 4 &&
                dataModel.SelectedLanguages != null && dataModel.SelectedLanguages.Count > 0)
            {
                isValidModel = true;
            }

            return isValidModel;
        }

        private static List<ValueMatchedItem> FindItemsByValue(FindByValueDataModel dataModel, out string errorLog)
        {
            errorLog = string.Empty;
            List<ValueMatchedItem> lstValueMatchedItems = null;
            List<Item> lstItemsToCheck = GetCheckListItems(dataModel.ParentItemId, dataModel.TargetTemplateId);

            if (lstItemsToCheck != null && lstItemsToCheck.Count > 0)
            {
                lstValueMatchedItems = new List<ValueMatchedItem>();
                int matchCondition = dataModel.MatchCondition;
                string keyword = dataModel.Keyword.Trim().ToLower();
                List<string> lstFields = null;
                Item itemByLanguage = null;

                foreach (Item item in lstItemsToCheck)
                {
                    lstFields = new List<string>();
                    Language currentLanguage = item.Language;
                    lstFields = GetValueMatchedFields(item, matchCondition, keyword);
                    lstValueMatchedItems = AddToMatchedList(item, lstValueMatchedItems, lstFields);

                    List<Language> lstOtherSelectedLanguages = dataModel.SelectedLanguages.Where(x => x != currentLanguage).ToList();

                    foreach (Language language in lstOtherSelectedLanguages)
                    {
                        lstFields = null;
                        itemByLanguage = SctHelper.GetItem(item.ID, language);
                        lstFields = GetValueMatchedFields(itemByLanguage, matchCondition, keyword);
                        lstValueMatchedItems = AddToMatchedList(itemByLanguage, lstValueMatchedItems, lstFields);
                    }
                }
            }
            else
            {
                errorLog = "No items found to search";
            }

            return lstValueMatchedItems;
        }

        private static List<string> GetValueMatchedFields(Item item, int matchCondition, string keyword)
        {
            List<string> lstFields = null;
            bool matchFound = false;

            if (item.Versions.Count > 0)
            {
                lstFields = new List<string>();
                item.Fields.ReadAll();

                foreach (Field field in item.Fields)
                {
                    matchFound = false;

                    switch (matchCondition)
                    {
                        case 1:
                            if (item.Fields[field.ID].Value.ToLower().Contains(keyword))
                            {
                                matchFound = true;
                            }
                            break;
                        case 2:
                            if (item.Fields[field.ID].Value.ToLower().StartsWith(keyword))
                            {
                                matchFound = true;
                            }
                            break;
                        case 3:
                            if (item.Fields[field.ID].Value.ToLower().EndsWith(keyword))
                            {
                                matchFound = true;
                            }
                            break;
                    }

                    if (matchFound)
                    {
                        lstFields.Add(field.DisplayName);
                    }
                }
            }

            return lstFields;
        }

        private static List<Item> GetCheckListItems(string parentItemId, string targetTemlateId)
        {
            List<Item> lstItemsToCheck = null;
            Item parentItem = SctHelper.GetItem(parentItemId);

            if (parentItem != null)
            {
                lstItemsToCheck = new List<Item>();

                ///include parent item in the checklist
                lstItemsToCheck.Add(parentItem);

                if (parentItem.HasChildren)
                {
                    if (!string.IsNullOrEmpty(targetTemlateId))
                    {
                        lstItemsToCheck.AddRange(parentItem.Axes.GetDescendants().Where(x => x.TemplateID == new ID(targetTemlateId)));
                    }
                    else
                    {
                        lstItemsToCheck.AddRange(parentItem.Axes.GetDescendants());
                    }
                }
            }

            return lstItemsToCheck;
        }

        private static List<ValueMatchedItem> AddToMatchedList(Item item, List<ValueMatchedItem> existingMatchedList, List<string> fields)
        {
            if (fields != null && fields.Count > 0)
            {
                existingMatchedList.Add(new ValueMatchedItem
                {
                    ItemPath = item.Paths.FullPath,
                    LanguageCode = item.Language.Name,
                    Fields = fields
                });
            }

            return existingMatchedList;
        }

        private static List<ValueMatchedItem> GetMatchLog(List<ValueMatchedItem> lstValueMatchedItems, List<Language> languages)
        {
            List<ValueMatchedItem> lstMatchLog = new List<ValueMatchedItem>();
            ValueMatchedItem valueMatchedItem = null;
            List<string> uniqueItemPaths = lstValueMatchedItems.Select(x => x.ItemPath).Distinct().ToList();
            StringBuilder sbLog = new StringBuilder();

            foreach (string itemPath in uniqueItemPaths)
            {
                foreach (Language language in languages)
                {
                    valueMatchedItem = lstValueMatchedItems.Where(x => x.ItemPath == itemPath && x.LanguageCode == language.Name).First();

                    if (valueMatchedItem != null)
                    {
                        sbLog.AppendLine("<strong>Language: " + language.Name + "</strong><br>");
                        sbLog.AppendLine(string.Join(", ", valueMatchedItem.Fields).TrimEnd(','));
                        sbLog.AppendLine("<br>");
                    }
                }

                lstMatchLog.Add(new ValueMatchedItem { ItemPath = itemPath, MatchLog = sbLog.ToString() });
                sbLog = new StringBuilder();
            }

            return lstMatchLog;
        }

        #endregion
    }
}

namespace SitecoreCustomTools.sitecore.admin.minions
{
    using Microsoft.VisualBasic;
    using Sitecore.Data;
    using Sitecore.Data.Fields;
    using Sitecore.Data.Items;
    using Sitecore.Globalization;
    using SitecoreCustomTools.Models;
    using SitecoreCustomTools.Utilities;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
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
                                result.TaskStatusMessage = "List of items whose field values have this keyword";
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

        [WebMethod]
        public static string ReplaceKeyword(FindByValueDataModel findByValueDataModel)
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
                                result.TaskStatusMessage = "List of items whose field values are updated with the new value";
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
                int taskId = dataModel.TaskId;
                List<Language> selectedLanguages = dataModel.SelectedLanguages;
                List<string> lstFields = null;
                Item itemByLanguage = null;
                bool replaceKeywordInContent = false;
                string replaceValue = dataModel.ReplaceValue;

                foreach (Item item in lstItemsToCheck)
                {
                    lstFields = new List<string>();
                    Language currentLanguage = item.Language;

                    if (selectedLanguages.Any(x => x.Name == currentLanguage.Name))
                    {
                        replaceKeywordInContent = taskId == 2;
                        lstFields = GetValueMatchedFields(item, matchCondition, keyword, replaceKeywordInContent, replaceValue);
                        lstValueMatchedItems = AddToMatchedList(item, lstValueMatchedItems, lstFields); 
                    }

                    List<Language> lstOtherSelectedLanguages = dataModel.SelectedLanguages.Where(x => x != currentLanguage).ToList();

                    foreach (Language language in lstOtherSelectedLanguages)
                    {
                        lstFields = null;
                        itemByLanguage = SctHelper.GetItem(item.ID, language);
                        replaceKeywordInContent = taskId == 2 && selectedLanguages.Any(x => x.Name == language.Name);
                        lstFields = GetValueMatchedFields(itemByLanguage, matchCondition, keyword, replaceKeywordInContent, replaceValue);
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

        private static List<string> GetValueMatchedFields(Item item, int matchCondition, string keyword, bool replaceKeywordInContent = false, string replaceWith = "")
        {
            List<string> lstFields = null;
            bool matchFound = false;
            string content = string.Empty;
            string newFieldValue = string.Empty;

            if (item.Versions.Count > 0)
            {
                keyword = keyword.ToLower();
                lstFields = new List<string>();
                item.Fields.ReadAll();

                foreach (Field field in item.Fields)
                {
                    matchFound = false;
                    content = item.Fields[field.ID].Value;

                    switch (matchCondition)
                    {
                        case 1:
                            if (content.IndexOf(keyword, 0, StringComparison.CurrentCultureIgnoreCase) != -1)
                            {
                                matchFound = true;

                                if (replaceKeywordInContent)
                                {
                                    newFieldValue = Microsoft.VisualBasic.Strings.Replace(content, keyword, replaceWith, 1, -1,CompareMethod.Text);
                                }
                            }
                            break;
                        case 2:
                            if (content.StartsWith(keyword, StringComparison.CurrentCultureIgnoreCase))
                            {
                                matchFound = true;

                                if (replaceKeywordInContent)
                                {
                                }
                            }
                            break;
                        case 3:
                            if (content.EndsWith(keyword, StringComparison.CurrentCultureIgnoreCase))
                            {
                                matchFound = true;

                                if (replaceKeywordInContent)
                                {
                                }
                            }
                            break;
                    }

                    if (matchFound)
                    {
                        lstFields.Add(field.DisplayName);

                        if (replaceKeywordInContent)
                        {
                            UpdateFieldValue(item, field.ID, newFieldValue);
                        }
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
                    valueMatchedItem = lstValueMatchedItems.Where(x => x.ItemPath == itemPath && x.LanguageCode == language.Name).FirstOrDefault();

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

        private static void UpdateFieldValue(Item sourceItem, ID fieldId, string newFieldValue)
        {
            SctHelper.UpdateFieldValues(sourceItem, new List<KeyValuePair<ID, string>> {
                                                    new KeyValuePair<ID, string> ( fieldId, newFieldValue )
                                                });
        }
        #endregion
    }
}
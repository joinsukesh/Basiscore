﻿
namespace Basiscore.Minions.sitecore.admin.minions
{
    using Basiscore.Minions;
    using Basiscore.Minions.Models;
    using Basiscore.Minions.Utilities;
    using Microsoft.VisualBasic;
    using Sitecore.Data;
    using Sitecore.Data.Fields;
    using Sitecore.Data.Items;
    using Sitecore.Globalization;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Web.Script.Serialization;
    using System.Web.Services;

    public partial class updatefields : System.Web.UI.Page
    {
        #region EVENTS

        protected void Page_Load(object sender, EventArgs e)
        {
            if (MinionHelper.IsUserLoggedIn())
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
                Response.Redirect(MinionConstants.Paths.LoginPagePath);
            }
        }

        [WebMethod]
        public static string ReplaceKeyword(FindByValueDataModel dataModel)
        {
            FindByValueReport result = new FindByValueReport();
            string error = "";
            string output = "";

            try
            {
                if (MinionHelper.IsUserLoggedIn())
                {
                    ///All update activies like replace/update should only be done based on field-id and not the name.
                    ///so as a double check, setting the input-type to 1 (just incase user has tampered it to any other value.
                    dataModel.TargetFieldInputType = 1;

                    if (IsValidModel(dataModel, out error))
                    {
                        List<ValueMatchedItem> lstValueMatchedItems = FindItemsByValue(dataModel, out error);

                        if (string.IsNullOrEmpty(error))
                        {
                            if (lstValueMatchedItems != null && lstValueMatchedItems.Count > 0)
                            {
                                result.TaskStatus = 1;
                                result.LstValueMatchedItems = GetMatchLog(lstValueMatchedItems, dataModel.SelectedLanguages);
                                result.TaskStatusMessage = "List of items whose field values are updated";
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
                        result.Error = error;
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
        public static string ReplaceKeywordForSpecifiedItems(FindByValueDataModel dataModel)
        {
            FindByValueReport result = new FindByValueReport();
            string error = "";
            string output = "";

            try
            {
                if (MinionHelper.IsUserLoggedIn())
                {
                    ///All update activies like replace/update should only be done based on field-id and not the name.
                    ///so as a double check, setting the input-type to 1 (just incase user has tampered it to any other value.
                    dataModel.TargetFieldInputType = 1;

                    if (IsValidModel(dataModel, out error))
                    {
                        List<ValueMatchedItem> lstValueMatchedItems = FindItemsByValue(dataModel, out error);

                        if (string.IsNullOrEmpty(error))
                        {
                            if (lstValueMatchedItems != null && lstValueMatchedItems.Count > 0)
                            {
                                result.TaskStatus = 1;
                                result.LstValueMatchedItems = GetMatchLog(lstValueMatchedItems, dataModel.SelectedLanguages);
                                result.TaskStatusMessage = "List of items whose field values are updated";
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
                        result.Error = error;
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
        public static string UpdateFieldValue(FindByValueDataModel dataModel)
        {
            FindByValueReport result = new FindByValueReport();
            string error = "";
            string output = "";

            try
            {
                if (MinionHelper.IsUserLoggedIn())
                {
                    ///All update activies like replace/update should only be done based on field-id and not the name.
                    ///so as a double check, setting the input-type to 1 (just incase user has tampered it to any other value.
                    dataModel.TargetFieldInputType = 1;

                    if (IsValidModel(dataModel, out error))
                    {
                        List<ValueMatchedItem> lstValueMatchedItems = FindItemsByValue(dataModel, out error);

                        if (string.IsNullOrEmpty(error))
                        {
                            if (lstValueMatchedItems != null && lstValueMatchedItems.Count > 0)
                            {
                                result.TaskStatus = 1;
                                result.LstValueMatchedItems = GetMatchLog(lstValueMatchedItems, dataModel.SelectedLanguages);
                                result.TaskStatusMessage = "List of items whose field values are updated";
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
                        result.Error = error;
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
        public static string UpdateFieldValueByItemPaths(FindByValueDataModel dataModel)
        {
            FindByValueReport result = new FindByValueReport();
            string error = "";
            string output = "";

            try
            {
                if (MinionHelper.IsUserLoggedIn())
                {
                    ///All update activies like replace/update should only be done based on field-id and not the name.
                    ///so as a double check, setting the input-type to 1 (just incase user has tampered it to any other value.
                    dataModel.TargetFieldInputType = 1;

                    if (IsValidModel(dataModel, out error))
                    {
                        List<ValueMatchedItem> lstValueMatchedItems = FindItemsByValue(dataModel, out error);

                        if (string.IsNullOrEmpty(error))
                        {
                            if (lstValueMatchedItems != null && lstValueMatchedItems.Count > 0)
                            {
                                result.TaskStatus = 1;
                                result.LstValueMatchedItems = GetMatchLog(lstValueMatchedItems, dataModel.SelectedLanguages);
                                result.TaskStatusMessage = "List of items whose field values are updated";
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
                        result.Error = error;
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
            chkLanguages.DataSource = MinionHelper.GetInstalledLanguages();
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
        private static bool IsValidModel(FindByValueDataModel dataModel, out string error)
        {
            error = string.Empty;

            if (dataModel != null)
            {
                if (dataModel.TaskId < 0 || dataModel.TaskId > 6)
                {
                    error = "Invalid Task; ";
                }

                if (dataModel.TaskId == 2 || dataModel.TaskId == 3 )
                {
                    if (string.IsNullOrEmpty(dataModel.ParentItemId) || !MinionHelper.IsValidID(dataModel.ParentItemId))
                    {
                        error += "Invalid Parent Item Id; ";
                    }
                }

                if (!string.IsNullOrEmpty(dataModel.TargetTemplateId) && !MinionHelper.IsValidID(dataModel.TargetTemplateId))
                {
                    error += "Invalid target template Id; ";
                }

                if (dataModel.TaskId == 3 || dataModel.TaskId == 5)
                {
                    string inputTargetFieldValue = dataModel.TargetFieldId;

                    if (string.IsNullOrEmpty(inputTargetFieldValue))
                    {
                        error += "Invalid target field Id/name; ";
                    }
                    else if (dataModel.TargetFieldInputType == 1 &&
                                !MinionHelper.IsValidID(inputTargetFieldValue))
                    {
                        error += "Invalid target field Id; ";
                    }
                }

                if (dataModel.TaskId == 2 || dataModel.TaskId == 6)
                {
                    if (string.IsNullOrEmpty(Convert.ToString(dataModel.Keyword).Trim()))
                    {
                        error += "Invalid search keyword; ";
                    }
                }

                if (dataModel.MatchCondition < 0 || dataModel.MatchCondition > 3)
                {
                    error += "Invalid input; ";
                }

                if (dataModel.TaskId == 2 || dataModel.TaskId == 3 ||
                    dataModel.TaskId == 5 || dataModel.TaskId == 6)
                {
                    if (dataModel.SelectedLanguages == null || dataModel.SelectedLanguages.Count == 0)
                    {
                        error += "Invalid target language; ";
                    }
                }
                
                if (dataModel.TaskId == 5 || dataModel.TaskId == 6)
                {
                    List<string> lstItemPaths = dataModel.GetItemPaths();

                    if (lstItemPaths == null || lstItemPaths.Count == 0)
                    {
                        error += "Invalid item paths; ";
                    }
                }
            }
            else
            {
                error = "Invalid input";
            }

            error = error.Trim().TrimEnd(';');
            return string.IsNullOrEmpty(error);
        }

        private static List<ValueMatchedItem> FindItemsByValue(FindByValueDataModel dataModel, out string errorLog)
        {
            errorLog = string.Empty;
            List<ValueMatchedItem> lstValueMatchedItems = null;
            List<Item> lstItemsToCheck = null;
            int taskId = dataModel.TaskId;

            if (taskId == 5 || taskId == 6)
            {
                lstItemsToCheck = MinionHelper.GetItemsFromPathOrId(dataModel.GetItemPaths());
            }
            else
            {
                lstItemsToCheck = GetCheckListItems(dataModel.ParentItemId, dataModel.TargetTemplateId);
            }

            if (lstItemsToCheck != null && lstItemsToCheck.Count > 0)
            {
                lstValueMatchedItems = new List<ValueMatchedItem>();
                List<string> lstFields = null;
                Item itemByLanguage = null;
                Language currentLanguage = null;
                List<Language> selectedLanguages = dataModel.SelectedLanguages;
                List<Language> lstOtherSelectedLanguages = null;

                foreach (Item item in lstItemsToCheck)
                {
                    lstFields = new List<string>();
                    currentLanguage = item.Language;

                    ///process the task wih the current language item
                    if (selectedLanguages.Any(x => x.Name == currentLanguage.Name))
                    {
                        lstFields = GetValueMatchedFields(item, dataModel);
                        lstValueMatchedItems = AddToMatchedList(item, lstValueMatchedItems, lstFields);
                    }

                    ///get languages other than the one processed above
                    lstOtherSelectedLanguages = selectedLanguages.Where(x => x != currentLanguage).ToList();

                    foreach (Language language in lstOtherSelectedLanguages)
                    {
                        lstFields = null;
                        itemByLanguage = MinionHelper.GetItem(item.ID, language);
                        lstFields = GetValueMatchedFields(itemByLanguage, dataModel);
                        lstValueMatchedItems = AddToMatchedList(itemByLanguage, lstValueMatchedItems, lstFields);
                    }
                }
            }
            else
            {
                errorLog = "No items found for these inputs";
            }

            return lstValueMatchedItems;
        }

        private static List<string> GetValueMatchedFields(Item item, FindByValueDataModel dataModel)
        {
            List<string> lstFields = null;
            bool matchFound = false;
            string content = string.Empty;
            string newFieldValue = string.Empty;
            List<KeyValuePair<ID, string>> lstItemFieldsToBeUpdated = new List<KeyValuePair<ID, string>>();
            List<string> lstUpdatedFieldNames = new List<string>();
            int taskId = dataModel.TaskId;
            string inputTargetFieldValue = dataModel.TargetFieldId;

            Field field = null;

            if (!string.IsNullOrEmpty(inputTargetFieldValue))
            {
                if (dataModel.TargetFieldInputType == 1 && MinionHelper.ItemHasField(item, new ID(inputTargetFieldValue)))
                {
                    field = item.Fields[new ID(inputTargetFieldValue)];
                }
                else if (MinionHelper.ItemHasField(item, inputTargetFieldValue))
                {
                    field = item.Fields[inputTargetFieldValue];
                }
            }

            if (taskId == 3 || taskId == 5)
            {
                ///if the task is to update field value
                if (field != null && field.ID != Sitecore.Data.ID.NewID && !field.ID.IsNull && MinionHelper.ItemHasField(item, field.ID))
                {
                    lstFields = new List<string>();

                    switch (dataModel.UpdateCondition)
                    {
                        case "1":
                            newFieldValue = dataModel.ReplaceValue + field.Value;
                            break;
                        case "2":
                            newFieldValue = field.Value + dataModel.ReplaceValue;
                            break;
                        case "3":
                            newFieldValue = dataModel.ReplaceValue;
                            break;
                    }

                    KeyValuePair<ID, string> kvp = new KeyValuePair<ID, string>(field.ID, newFieldValue);
                    MinionHelper.UpdateFieldValues(item, new List<KeyValuePair<ID, string>> { kvp }, dataModel.CreateVersion);
                    lstFields.Add(field.DisplayName);
                }
            }
            else
            {
                if (item.Versions.Count > 0)
                {
                    lstFields = new List<string>();
                    int matchCondition = dataModel.MatchCondition;
                    string keyword = dataModel.Keyword;
                    bool replaceKeywordInContent = (taskId == 2 || taskId == 6); ///if task is to replace keyword in content
                    string replaceWith = dataModel.ReplaceValue;


                    if (field != null)
                    {
                        content = item.Fields[field.ID].Value;
                        matchFound = IsMatchFound(matchCondition, content, keyword, replaceWith, replaceKeywordInContent, out newFieldValue);

                        if (matchFound)
                        {
                            if (replaceKeywordInContent)
                            {
                                ///collect field whose value is to be updated
                                lstItemFieldsToBeUpdated.Add(new KeyValuePair<ID, string>(field.ID, newFieldValue));
                                lstUpdatedFieldNames.Add(field.DisplayName);
                            }
                            else
                            {
                                lstFields.Add(field.DisplayName);
                            }
                        }
                    }
                    else
                    {
                        ///check all the fields only when user has not provided a target field.
                        if (string.IsNullOrEmpty(inputTargetFieldValue))
                        {
                            List<TemplateFieldItem> lstNonSystemTemplateFields = MinionHelper.GetTemplateFields(item, false);

                            if (lstNonSystemTemplateFields != null && lstNonSystemTemplateFields.Count > 0)
                            {
                                foreach (TemplateFieldItem tfi in lstNonSystemTemplateFields)
                                {
                                    matchFound = false;
                                    content = item.Fields[tfi.ID].Value;
                                    matchFound = IsMatchFound(matchCondition, content, keyword, replaceWith, replaceKeywordInContent, out newFieldValue);

                                    if (matchFound)
                                    {
                                        if (replaceKeywordInContent)
                                        {
                                            ///collect all fields whose values are to be updated
                                            lstItemFieldsToBeUpdated.Add(new KeyValuePair<ID, string>(tfi.ID, newFieldValue));
                                            lstUpdatedFieldNames.Add(tfi.DisplayName);
                                        }
                                        else
                                        {
                                            lstFields.Add(tfi.DisplayName);
                                        }
                                    }
                                }
                            }
                        }
                    }

                    ///update field values
                    if (lstItemFieldsToBeUpdated.Count > 0)
                    {
                        MinionHelper.UpdateFieldValues(item, lstItemFieldsToBeUpdated, dataModel.CreateVersion);
                        ///add to list only after update is complete
                        lstFields.AddRange(lstUpdatedFieldNames);
                    }
                }
            }

            return lstFields;
        }

        private static List<Item> GetCheckListItems(string parentItemId, string targetTemlateId)
        {
            List<Item> lstItemsToCheck = null;
            Item parentItem = MinionHelper.GetItem(parentItemId, "");

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
                    ItemId = item.ID.ToString(),
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

                lstMatchLog.Add(new ValueMatchedItem { ItemId = valueMatchedItem.ItemId, ItemPath = itemPath, MatchLog = sbLog.ToString() });
                sbLog = new StringBuilder();
            }

            return lstMatchLog;
        }

        private static bool IsMatchFound(int matchCondition, string sourceContent, string keyword, string replaceWith, bool replaceKeyword, out string updatedFieldValue)
        {
            bool matchFound = false;
            updatedFieldValue = string.Empty;

            switch (matchCondition)
            {
                case 1:
                    if (sourceContent.IndexOf(keyword, 0, StringComparison.CurrentCultureIgnoreCase) != -1)
                    {
                        matchFound = true;

                        if (replaceKeyword)
                        {
                            updatedFieldValue = Microsoft.VisualBasic.Strings.Replace(sourceContent, keyword, replaceWith, 1, -1, CompareMethod.Text);
                        }
                    }
                    break;
                case 2:
                    if (sourceContent.StartsWith(keyword, StringComparison.CurrentCultureIgnoreCase))
                    {
                        matchFound = true;

                        if (replaceKeyword)
                        {
                        }
                    }
                    break;
                case 3:
                    if (sourceContent.EndsWith(keyword, StringComparison.CurrentCultureIgnoreCase))
                    {
                        matchFound = true;

                        if (replaceKeyword)
                        {
                        }
                    }
                    break;
            }

            return matchFound;
        }

        #endregion
    }
}
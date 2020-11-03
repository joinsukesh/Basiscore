
namespace Basiscore.Minions.sitecore.admin.minions
{
    using Basiscore.Minions;
    using Basiscore.Minions.Models;
    using Basiscore.Minions.Utilities;
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

    public partial class fieldvalues : System.Web.UI.Page
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
        public static string FindItems(FindByValueDataModel dataModel)
        {
            FindByValueReport result = new FindByValueReport();
            string error = "";
            string output = "";

            try
            {
                if (MinionHelper.IsUserLoggedIn())
                {
                    if (IsValidModel(dataModel, out error))
                    {
                        List<ValueMatchedItem> lstValueMatchedItems = FindItemsByValue(dataModel, out error);

                        if (string.IsNullOrEmpty(error))
                        {
                            if (lstValueMatchedItems != null && lstValueMatchedItems.Count > 0)
                            {
                                result.TaskStatus = 1;
                                result.LstValueMatchedItems = GetMatchLog(lstValueMatchedItems, dataModel.SelectedLanguage);
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
        public static string GetItemsAndFieldValues(FindByValueDataModel dataModel)
        {
            FindByValueReport result = new FindByValueReport();
            string error = "";
            string output = "";
            string fieldName = "";

            try
            {
                if (MinionHelper.IsUserLoggedIn())
                {
                    if (IsValidModel(dataModel, out error))
                    {
                        List<ValueMatchedItem> lstValueMatchedItems = GetItemsAndFieldValuesByField(dataModel, out fieldName, out error);

                        if (string.IsNullOrEmpty(error))
                        {
                            result.ColumnName = fieldName;

                            if (lstValueMatchedItems != null && lstValueMatchedItems.Count > 0)
                            {
                                result.TaskStatus = 1;
                                result.LstValueMatchedItems = lstValueMatchedItems;
                                result.TaskStatusMessage = "List of items with their field values for the field - " + fieldName;
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
            ddlLanguages.DataSource = MinionHelper.GetInstalledLanguages();
            ddlLanguages.DataTextField = "Value";
            ddlLanguages.DataValueField = "Key";
            ddlLanguages.DataBind();
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
                if (!(dataModel.TaskId == 1 || dataModel.TaskId == 4))
                {
                    error = "Invalid Task; ";
                }

                if (string.IsNullOrEmpty(dataModel.ParentItemId) || !MinionHelper.IsValidID(dataModel.ParentItemId))
                {
                    error += "Invalid Parent Item Id; ";
                }

                if (!string.IsNullOrEmpty(dataModel.TargetTemplateId) && !MinionHelper.IsValidID(dataModel.TargetTemplateId))
                {
                    error += "Invalid target template Id; ";
                }

                if (dataModel.TaskId == 4)
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

                if (dataModel.TaskId == 1)
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

                if (dataModel.SelectedLanguage == null)
                {
                    error += "Invalid target language; ";
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
            string database = dataModel.SourceDatabaseName;
            lstItemsToCheck = GetCheckListItems(dataModel.ParentItemId, dataModel.TargetTemplateId, database);

            if (lstItemsToCheck != null && lstItemsToCheck.Count > 0)
            {
                lstValueMatchedItems = new List<ValueMatchedItem>();
                List<string> lstFields = null;
                Item itemByLanguage = null;
                Language selectedLanguage = dataModel.SelectedLanguage;

                foreach (Item item in lstItemsToCheck)
                {
                    lstFields = new List<string>();
                    itemByLanguage = MinionHelper.GetItem(item.ID, selectedLanguage, database);
                    lstFields = GetValueMatchedFields(itemByLanguage, dataModel);
                    lstValueMatchedItems = AddToMatchedList(itemByLanguage, lstValueMatchedItems, lstFields);
                }
            }
            else
            {
                errorLog = "No items found for these inputs";
            }

            return lstValueMatchedItems;
        }

        private static List<ValueMatchedItem> GetItemsAndFieldValuesByField(FindByValueDataModel dataModel, out string fieldName, out string errorLog)
        {
            fieldName = string.Empty;
            errorLog = string.Empty;
            List<ValueMatchedItem> lstValueMatchedItems = null;
            string database = dataModel.SourceDatabaseName;
            List<Item> lstItemsToCheck = GetCheckListItems(dataModel.ParentItemId, dataModel.TargetTemplateId, database);

            if (lstItemsToCheck != null && lstItemsToCheck.Count > 0)
            {
                lstValueMatchedItems = new List<ValueMatchedItem>();
                Item itemByLanguage = null;
                int taskId = dataModel.TaskId;
                int targetFieldInputType = dataModel.TargetFieldInputType;
                Language selectedLanguage = dataModel.SelectedLanguage;
                string inputTargetFieldValue = dataModel.TargetFieldId;

                if (targetFieldInputType == 1)
                {
                    Item fieldItem = MinionHelper.GetItem(inputTargetFieldValue);
                    fieldName = fieldItem != null ? fieldItem.DisplayName : "FIELD";
                }
                else
                {
                    fieldName = !string.IsNullOrEmpty(inputTargetFieldValue) ? inputTargetFieldValue : "FIELD";
                }

                ID fieldId = targetFieldInputType == 1 ? new ID(inputTargetFieldValue) : null;

                foreach (Item item in lstItemsToCheck)
                {
                    bool itemHasField = targetFieldInputType == 1 ? MinionHelper.ItemHasField(item, fieldId) : MinionHelper.ItemHasField(item, fieldName);

                    if (itemHasField)
                    {
                        itemByLanguage = MinionHelper.GetItem(item.ID, selectedLanguage, database);
                        lstValueMatchedItems.Add(new ValueMatchedItem
                        {
                            ItemId = itemByLanguage.ID.ToString(),
                            ItemPath = itemByLanguage.Paths.FullPath,
                            MatchLog = targetFieldInputType == 1 ? itemByLanguage.Fields[fieldId].Value : itemByLanguage.Fields[fieldName].Value
                        });
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

            if (taskId == 1 || taskId == 4)
            {
                if (item.Versions.Count > 0)
                {
                    lstFields = new List<string>();
                    int matchCondition = dataModel.MatchCondition;
                    string keyword = dataModel.Keyword;
                    string replaceWith = dataModel.ReplaceValue;

                    if (field != null)
                    {
                        content = item.Fields[field.ID].Value;
                        matchFound = IsMatchFound(matchCondition, content, keyword);

                        if (matchFound)
                        {
                            lstFields.Add(field.DisplayName);
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
                                    matchFound = IsMatchFound(matchCondition, content, keyword);

                                    if (matchFound)
                                    {
                                        lstFields.Add(tfi.DisplayName);
                                    }
                                }
                            }
                        }
                    }                    
                }
            }

            return lstFields;
        }

        private static List<Item> GetCheckListItems(string parentItemId, string targetTemlateId, string database)
        {
            List<Item> lstItemsToCheck = null;
            Item parentItem = MinionHelper.GetItem(parentItemId, null, database);

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

        private static List<ValueMatchedItem> GetMatchLog(List<ValueMatchedItem> lstValueMatchedItems, Language targetLanguage)
        {
            List<ValueMatchedItem> lstMatchLog = new List<ValueMatchedItem>();
            ValueMatchedItem valueMatchedItem = null;
            List<string> uniqueItemPaths = lstValueMatchedItems.Select(x => x.ItemPath).Distinct().ToList();
            StringBuilder sbLog = new StringBuilder();

            foreach (string itemPath in uniqueItemPaths)
            {
                valueMatchedItem = lstValueMatchedItems.Where(x => x.ItemPath == itemPath && x.LanguageCode == targetLanguage.Name).FirstOrDefault();

                if (valueMatchedItem != null)
                {
                    sbLog.AppendLine("<strong>Language: " + targetLanguage.Name + "</strong><br>");
                    sbLog.AppendLine(string.Join(", ", valueMatchedItem.Fields).TrimEnd(','));
                    sbLog.AppendLine("<br>");
                }

                lstMatchLog.Add(new ValueMatchedItem { ItemId = valueMatchedItem.ItemId, ItemPath = itemPath, MatchLog = sbLog.ToString() });
                sbLog = new StringBuilder();
            }

            return lstMatchLog;
        }

        private static bool IsMatchFound(int matchCondition, string sourceContent, string keyword)
        {
            bool matchFound = false;

            switch (matchCondition)
            {
                case 1:
                    if (sourceContent.IndexOf(keyword, 0, StringComparison.CurrentCultureIgnoreCase) != -1)
                    {
                        matchFound = true;
                    }
                    break;
                case 2:
                    if (sourceContent.StartsWith(keyword, StringComparison.CurrentCultureIgnoreCase))
                    {
                        matchFound = true;
                    }
                    break;
                case 3:
                    if (sourceContent.EndsWith(keyword, StringComparison.CurrentCultureIgnoreCase))
                    {
                        matchFound = true;
                    }
                    break;
            }

            return matchFound;
        }

        #endregion
    }
}

namespace Basiscore.Minions.sitecore.admin.minions
{
    using Basiscore.Minions.Models;
    using Basiscore.Minions.Utilities;
    using Sitecore;
    using Sitecore.Data.Items;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Text;

    public partial class importexceldata : System.Web.UI.Page
    {
        #region CONFIGURATIONS

        private string[] ValidFileExtensions = { ".xlsx" };

        ///The names defined here should match with the column names of the excel sheet.
        public struct ExcelColumnNames
        {
            public static string ITEM_NAME = "ITEM_NAME";
            public static string ITEM_TEMPLATE = "ITEM_TEMPLATE";
            public static string PARENT_ITEM = "PARENT_ITEM";
        }

        #endregion

        #region VARIABLES

        private List<ImportExcelDataItemInfo> lstInvalidItemInfo = new List<ImportExcelDataItemInfo>();
        private int itemsCreated = 0;
        private int itemsUpdated = 0;

        #endregion

        #region EVENTS

        protected void Page_PreRender(object sender, EventArgs e)
        {
            ViewState[MinionConstants.Timestamp] = Session[MinionConstants.Timestamp];
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (MinionHelper.IsUserLoggedIn())
            {
                try
                {
                    if (!IsPostBack)
                    {
                        BindData();
                        Session[MinionConstants.Timestamp] = Server.UrlEncode(DateTime.Now.ToString());
                    }
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

        protected void btnImportData_Click(object sender, EventArgs e)
        {
            if (MinionHelper.IsUserLoggedIn())
            {
                if (System.Convert.ToString(Session[MinionConstants.Timestamp]) == System.Convert.ToString(ViewState[MinionConstants.Timestamp]))
                {
                    ViewState[MinionConstants.Timestamp] = DateTime.Now;
                    string statusMessage = string.Empty;
                    string error = string.Empty;
                    string endRowInput = txtEndAtRow.Text.Trim();
                    int startRowInExcelSheet = 2;
                    int endRowInExcelSheet = -1;
                    bool updateFieldValuesIfItemExists = false;
                    ltStatus.Text = "";
                    tblResult.InnerHtml = "";

                    try
                    {
                        if (IsValidModel(out error))
                        {
                            ///in the excel file, the user input data starts from row 2
                            string selectedLanguage = ddlLanguages.SelectedValue;
                            startRowInExcelSheet = MainUtil.GetInt(txtStartFromRow.Text.Trim(), 2);
                            endRowInExcelSheet = MainUtil.GetInt(endRowInput, -1);
                            updateFieldValuesIfItemExists = chkUpdateFieldValues.Checked;
                            DataTable dtExcel = MinionHelper.GetExcelData(fupExcel.PostedFile.InputStream);

                            if (dtExcel != null && dtExcel.Rows.Count > 0)
                            {
                                int excelRowsCount = dtExcel.Rows.Count;

                                ///If start & end rows are not set, all rows in excel will be processed.
                                ///If start row is not set, it is set to first data row (2) in excel as default.
                                ///If end row is not set, it is set to the last data row in excel.
                                ///If start & end rows are set, all rows in between and including them, will be processed.
                                endRowInExcelSheet = endRowInExcelSheet == -1 ? excelRowsCount + 1 : endRowInExcelSheet;

                                ///check if input row numbers are valid
                                if (((startRowInExcelSheet - 1) <= excelRowsCount) && ((endRowInExcelSheet - 1) <= excelRowsCount))
                                {
                                    List<ImportExcelDataItemInfo> lstItemsToBeCreated = null;
                                    List<ImportExcelDataItemInfo> lstItemsToBeUpdated = null;
                                    List<ImportExcelDataItemInfo> lstParentItemsNotFound = null;

                                    CollectItemsToBeProcessed(dtExcel, updateFieldValuesIfItemExists, out lstItemsToBeCreated,
                                        out lstItemsToBeUpdated, out lstParentItemsNotFound, startRowInExcelSheet, endRowInExcelSheet);

                                    CreateItems(lstItemsToBeCreated, selectedLanguage);

                                    UpdateItems(lstItemsToBeUpdated, selectedLanguage);

                                    RecheckForItemsWithInvalidParent(lstParentItemsNotFound, updateFieldValuesIfItemExists, selectedLanguage);

                                    ///if there are any invalid items, sort them by row number
                                    lstInvalidItemInfo = (lstInvalidItemInfo != null && lstInvalidItemInfo.Count > 0) ? lstInvalidItemInfo.OrderBy(x => x.ExcelSheetRowNumber).ToList() : lstInvalidItemInfo;
                                }
                                else
                                {
                                    error = "ERROR: Invalid row numbers";
                                }
                            }
                            else
                            {
                                error = "ERROR: No data available in the Excel sheet";
                            }
                        }
                        else
                        {
                            error = "ERROR:<br>" + error;
                        }
                    }
                    catch (Exception ex)
                    {
                        error = "ERROR: " + ex.Message;
                    }

                    if (!string.IsNullOrEmpty(error))
                    {
                        ltStatus.Text = "<span style=\"font-weight:bold;color:red;\">" + error + "</span>";
                    }
                    else
                    {
                        if (lstInvalidItemInfo != null && lstInvalidItemInfo.Count > 0)
                        {
                            if (updateFieldValuesIfItemExists)
                            {
                                ltStatus.Text = "<span style=\"font-weight:bold;color:red;\">THE FOLLOWING ITEM(S) COULD NOT BE CREATED/UPDATED.</span>";
                            }
                            else
                            {
                                ltStatus.Text = "<span style=\"font-weight:bold;color:red;\">THE FOLLOWING ITEM(S) COULD NOT BE CREATED.</span>";
                            }

                            tblResult.InnerHtml = GetStatusTable(lstInvalidItemInfo);
                        }
                        else
                        {
                            ///success
                            if (startRowInExcelSheet == endRowInExcelSheet)
                            {
                                statusMessage = "PROCESSED ROW: " + startRowInExcelSheet + "<br>";
                            }
                            else
                            {
                                if (string.IsNullOrEmpty(endRowInput))
                                {
                                    statusMessage = "PROCESSED ROWS:  From row " + startRowInExcelSheet + " until the last row.<br>";
                                }
                                else
                                {
                                    statusMessage = "PROCESSED ROWS:  From row " + startRowInExcelSheet + " to row " + endRowInExcelSheet + ".<br>";
                                }
                            }

                            statusMessage += "ITEMS CREATED:  " + itemsCreated + "<br>";
                            statusMessage += "ITEMS UPDATED:  " + itemsUpdated;
                            ltStatus.Text = "<span style=\"font-weight:bold;color:forestgreen;\">" + statusMessage + "</span>";
                            tblResult.InnerHtml = "";
                        }
                    }

                    hdnPostbackComplete.Value = "1";
                    Session[MinionConstants.Timestamp] = Server.UrlEncode(DateTime.Now.ToString());
                }
                else
                {
                    Response.Redirect(Request.RawUrl);
                }
            }
            else
            {
                Response.Redirect(MinionConstants.Paths.LoginPagePath);
            }
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

        private bool IsValidModel(out string error)
        {
            int errorCount = 0;
            StringBuilder sbError = new StringBuilder();
            error = "";

            if (!MinionHelper.IsValidFile(fupExcel.PostedFile.FileName, ValidFileExtensions))
            {
                errorCount++;
                sbError.AppendLine("Invalid file<br>");
            }

            int startRow = MainUtil.GetInt(txtStartFromRow.Text.Trim(), 2);

            if (startRow < 2)
            {
                errorCount++;
                sbError.AppendLine("Invalid start row<br>");
            }

            string endRowInput = txtEndAtRow.Text.Trim();

            if (!string.IsNullOrEmpty(endRowInput))
            {
                int endRow = MainUtil.GetInt(endRowInput, -1);

                if (startRow > endRow || endRow < 2)
                {
                    errorCount++;
                    sbError.AppendLine("Invalid end row<br>");
                }
            }

            error = sbError.ToString();
            return errorCount == 0;
        }

        private Item CreateItem(ImportExcelDataItemInfo itemToBeCreated, string language)
        {
            ///pass the key-value list to create item and its fields
            return MinionHelper.CreateItem(itemToBeCreated.ItemName, itemToBeCreated.ParentItem,
                itemToBeCreated.ItemTemplate, itemToBeCreated.FieldValueCollection, language, itemToBeCreated.BranchItem);
        }

        private void UpdateItem(ImportExcelDataItemInfo itemToBeUpdated, string language)
        {
            ///pass the key-value list to create item and its fields
            MinionHelper.UpdateFieldValues(itemToBeUpdated.CurrentItem, itemToBeUpdated.FieldValueCollection, true, language);
        }

        private void RecheckForItemsWithInvalidParent(List<ImportExcelDataItemInfo> lstParentItemsNotFound, bool updateFieldValuesIfItemExists, string language)
        {
            List<ImportExcelDataItemInfo> lstItemsToCheckInThisIteration = new List<ImportExcelDataItemInfo>();
            List<ImportExcelDataItemInfo> newList = new List<ImportExcelDataItemInfo>();

            if (lstParentItemsNotFound != null && lstParentItemsNotFound.Count > 0)
            {
                int count = 0;
                lstItemsToCheckInThisIteration = lstParentItemsNotFound;

                while (count < lstParentItemsNotFound.Count)
                {
                    foreach (ImportExcelDataItemInfo iteminfo in lstItemsToCheckInThisIteration)
                    {
                        if (iteminfo.ItemToCreateExists && updateFieldValuesIfItemExists)
                        {
                            UpdateItem(iteminfo, language);
                            itemsUpdated++;
                        }
                        else if (!iteminfo.ItemToCreateExists && iteminfo.ParentItem != null)
                        {
                            CreateItem(iteminfo, language);
                            itemsCreated++;
                        }
                        else if (count == lstParentItemsNotFound.Count - 1 && !iteminfo.ItemToCreateExists && iteminfo.ParentItem == null)
                        {
                            ///if it is the last iteration in loop, then add invalid items to the list
                            iteminfo.FailureReason = "Invalid parent item";
                            lstInvalidItemInfo.Add(iteminfo);
                        }
                        else
                        {
                            if (!newList.Any(x => x.ItemName == iteminfo.ItemName && x.ParentItem.Paths.FullPath == iteminfo.ParentItem.Paths.FullPath))
                            {
                                newList.Add(iteminfo);
                            }
                        }
                    }

                    lstItemsToCheckInThisIteration = newList;
                    count++;
                }
            }
        }

        private void CollectItemsToBeProcessed(DataTable dtExcel, bool updateFieldValuesIfItemExists,
            out List<ImportExcelDataItemInfo> lstItemsToBeCreated, out List<ImportExcelDataItemInfo> lstItemsToBeUpdated,
            out List<ImportExcelDataItemInfo> lstParentItemsNotFound, int startRowInExcelSheet, int endRowInExcelSheet)
        {
            lstItemsToBeCreated = new List<ImportExcelDataItemInfo>();
            lstItemsToBeUpdated = new List<ImportExcelDataItemInfo>();
            lstInvalidItemInfo = new List<ImportExcelDataItemInfo>();
            lstParentItemsNotFound = new List<ImportExcelDataItemInfo>();
            List<KeyValuePair<string, string>> fieldValueCollection = null;
            int columnCount = dtExcel.Columns.Count;

            for (int rowIndex = startRowInExcelSheet - 2; rowIndex <= endRowInExcelSheet - 2; rowIndex++)
            {
                if (IsValidRow(dtExcel.Rows[rowIndex], columnCount))
                {
                    ImportExcelDataItemInfo itemInfo = new ImportExcelDataItemInfo(System.Convert.ToString(dtExcel.Rows[rowIndex][ExcelColumnNames.ITEM_NAME]),
                                System.Convert.ToString(dtExcel.Rows[rowIndex][ExcelColumnNames.ITEM_TEMPLATE]),
                                System.Convert.ToString(dtExcel.Rows[rowIndex][ExcelColumnNames.PARENT_ITEM]));

                    itemInfo.ExcelSheetRowNumber = rowIndex + 2;

                    if (MinionHelper.IsValidItemName(itemInfo.ItemName))
                    {
                        fieldValueCollection = new List<KeyValuePair<string, string>>();

                        for (int colIndex = 3; colIndex < dtExcel.Columns.Count; colIndex++)
                        {
                            fieldValueCollection.Add(new KeyValuePair<string, string>(dtExcel.Columns[colIndex].ColumnName, System.Convert.ToString(dtExcel.Rows[rowIndex][colIndex])));
                        }

                        itemInfo.FieldValueCollection = fieldValueCollection;

                        if (itemInfo.ItemToCreateExists)
                        {
                            if (updateFieldValuesIfItemExists)
                            {
                                lstItemsToBeUpdated.Add(itemInfo);
                            }
                        }
                        else
                        {
                            if (itemInfo.ItemTemplate != null || itemInfo.BranchItem != null)
                            {
                                if (itemInfo.ParentItem != null)
                                {
                                    lstItemsToBeCreated.Add(itemInfo);
                                }
                                else
                                {
                                    lstParentItemsNotFound.Add(itemInfo);
                                }
                            }
                            else
                            {
                                itemInfo.FailureReason = "Invalid template";
                                lstInvalidItemInfo.Add(itemInfo);
                            }
                        }
                    }
                    else
                    {
                        itemInfo.FailureReason = "Invalid item name";
                        lstInvalidItemInfo.Add(itemInfo);
                    }
                }
            }
        }

        private void CreateItems(List<ImportExcelDataItemInfo> lstItemsToBeCreated, string language)
        {
            if (lstItemsToBeCreated != null && lstItemsToBeCreated.Count > 0)
            {
                foreach (ImportExcelDataItemInfo itemInfo in lstItemsToBeCreated)
                {
                    try
                    {
                        CreateItem(itemInfo, language);
                        itemsCreated++;
                    }
                    catch (Exception ex)
                    {
                        itemInfo.FailureReason = ex.Message;
                        lstInvalidItemInfo.Add(itemInfo);
                    }
                }
            }
        }

        private void UpdateItems(List<ImportExcelDataItemInfo> lstItemsToBeUpdated, string language)
        {
            if (lstItemsToBeUpdated != null && lstItemsToBeUpdated.Count > 0)
            {
                foreach (ImportExcelDataItemInfo itemInfo in lstItemsToBeUpdated)
                {
                    try
                    {
                        UpdateItem(itemInfo, language);
                        itemsUpdated++;
                    }
                    catch (Exception ex)
                    {
                        itemInfo.FailureReason = ex.Message;
                        lstInvalidItemInfo.Add(itemInfo);
                    }
                }
            }
        }

        /// <summary>
        /// if all the cells in a row are empty, then it is an invalid row
        /// </summary>
        /// <param name="row"></param>
        /// <param name="columnCount"></param>
        /// <returns></returns>
        private bool IsValidRow(DataRow row, int columnCount)
        {
            bool isValidRow = false;

            for (int index = 0; index < columnCount; index++)
            {
                if (!string.IsNullOrEmpty(System.Convert.ToString(row[index])))
                {
                    isValidRow = true;
                    break;
                }
            }

            return isValidRow;
        }

        private string GetStatusTable(List<ImportExcelDataItemInfo> lstInvalidItemInfo)
        {
            StringBuilder sb = new StringBuilder();
            int index = 1;
            sb.AppendLine("<table class=\"table table-striped table-bordered table-condensed\">");
            sb.AppendLine("<thead>");
            sb.AppendLine("<tr>");
            sb.AppendLine("<th style=\"width:10%\">#</th>");
            sb.AppendLine("<th style=\"width:30%\">ITEM NAME</th>");
            sb.AppendLine("<th style=\"width:10%\">ROW NO.</th>");
            sb.AppendLine("<th style=\"width:50%\">REASON</th>");
            sb.AppendLine("</tr>");
            sb.AppendLine("</thead>");

            sb.AppendLine("<tbody>");

            foreach (ImportExcelDataItemInfo itemInfo in lstInvalidItemInfo)
            {
                sb.AppendLine("<tr>");
                sb.AppendLine("<td>" + index + "</td>");
                sb.AppendLine("<td>" + itemInfo.ITEM_NAME + "</td>");
                sb.AppendLine("<td>" + itemInfo.ExcelSheetRowNumber + "</td>");
                sb.AppendLine("<td>" + itemInfo.FailureReason + "</td>");
                sb.AppendLine("</tr>");
                index++;
            }

            sb.AppendLine("</tbody>");
            sb.AppendLine("</table>");
            return sb.ToString();
        }

        #endregion
    }
}
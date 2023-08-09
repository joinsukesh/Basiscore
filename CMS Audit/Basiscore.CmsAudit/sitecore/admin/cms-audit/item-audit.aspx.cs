namespace Basiscore.CmsAudit.sitecore.admin.cms_audit
{
    using Basiscore.CmsAudit.Extensions;
    using Basiscore.CmsAudit.Models;
    using Basiscore.CmsAudit.Services;
    using Basiscore.CmsAudit.Utilities;
    using Sitecore.Diagnostics;
    using Sitecore.Globalization;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Text;

    public partial class item_audit : System.Web.UI.Page
    {
        private string SortDirection
        {
            get { return ViewState[Constants.SortDirection] != null ? ViewState[Constants.SortDirection].ToString() : Constants.ASC; }
            set { ViewState[Constants.SortDirection] = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (SitecoreUtility.IsAdministrator())
                {
                    if (!IsPostBack)
                    {
                        BindLanguages();
                    }
                }
                else
                {
                    Response.Redirect(Constants.LoginPagePath);
                }
            }
            catch (Exception ex)
            {
                Log.Error(Constants.ModuleName, ex, Constants.ModuleName);
                spError.Text = ex.Message;
            }
        }

        protected void btnIalSubmit_Click(object sender, EventArgs e)
        {
            try
            {
                if (SitecoreUtility.IsAdministrator())
                {
                    BindLogsData(true);
                }
                else
                {
                    Response.Redirect(Constants.LoginPagePath);
                }
            }
            catch (Exception ex)
            {
                Log.Error(Constants.ModuleName, ex, Constants.ModuleName);
                spError.Text = ex.Message;
            }            
        }

        protected void gvIal_PageIndexChanging(object sender, System.Web.UI.WebControls.GridViewPageEventArgs e)
        {
            try
            {
                gvIal.PageIndex = e.NewPageIndex;
                BindLogsData(true);
            }
            catch (Exception ex)
            {
                Log.Error(Constants.ModuleName, ex, Constants.ModuleName);
                spError.Text = ex.Message;
            }
        }

        protected void gvIal_Sorting(object sender, System.Web.UI.WebControls.GridViewSortEventArgs e)
        {
            try
            {
                BindLogsData(true, e.SortExpression);
            }
            catch (Exception ex)
            {
                Log.Error(Constants.ModuleName, ex, Constants.ModuleName);
                spError.Text = ex.Message;
            }
        }

        private void BindLanguages()
        {
            List<Language> languages = SitecoreUtility.GetSystemLanguages();

            if (languages != null && languages.Count > 0)
            {
                DataTable dt = new DataTable();
                string col_DisplayName = "DisplayName";
                string col_Name = "Name";
                dt.Columns.Add(col_DisplayName);
                dt.Columns.Add(col_Name);
                DataRow dr = null;

                foreach (Language language in languages)
                {
                    dr = dt.NewRow();
                    dr[col_DisplayName] = language.GetDisplayName();
                    dr[col_Name] = language.Name;
                    dt.Rows.Add(dr);
                }

                ddlLanguages.DataSource = dt;
                ddlLanguages.DataValueField = col_Name;
                ddlLanguages.DataTextField = col_DisplayName;
                ddlLanguages.DataBind();
            }
        }

        private void BindLogsData(bool isPostBack, string sortExpression = null)
        {
            DataTable dt = null;

            if (isPostBack)
            {
                ItemAuditLogRequest itemAuditLogRequest = new ItemAuditLogRequest();
                itemAuditLogRequest.LogType = Convert.ToInt32(ddlLogTypes.SelectedValue);
                itemAuditLogRequest.FromDateString = txtFromDate_Ial.Text;
                itemAuditLogRequest.ToDateString = txtToDate_Ial.Text;
                itemAuditLogRequest.ItemId = txtItemId.Text;
                itemAuditLogRequest.ActionedBy = txtUsername.Text;
                itemAuditLogRequest.ItemLanguage = ddlLanguages.SelectedValue;
                dt = GetAuditData(itemAuditLogRequest);

                if (dt != null && dt.Rows.Count > 0)
                {
                    if (sortExpression != null)
                    {
                        DataView dv = dt.AsDataView();
                        this.SortDirection = this.SortDirection == Constants.ASC ? Constants.DESC : Constants.ASC;

                        dv.Sort = sortExpression + " " + this.SortDirection;
                        gvIal.DataSource = dv; 
                    }
                    else
                    {
                        gvIal.DataSource = dt;
                    }
                }
                else
                {
                    dt = GetAuditLogsTableSchema();
                    dt.Rows.Add();
                    gvIal.DataSource = dt;
                }

                hdnIsPostBack.Value = Constants.One;
            }
            else
            {
                dt = GetAuditLogsTableSchema();
                dt.Rows.Add();
                gvIal.DataSource = dt;
            }

            gvIal.DataBind();
            upIal.Update();
        }

        private static DataTable GetAuditData(ItemAuditLogRequest itemAuditLogRequest)
        {
            DataTable dtLogs = GetAuditLogsTableSchema();

            DataTable dt = new DbService().GetItemAuditLogs(itemAuditLogRequest);

            if (dt != null && dt.Rows.Count > 0)
            {
                List<AuditLog_Item> lstAuditLogs = dt.ToList<AuditLog_Item>();

                if (lstAuditLogs != null && lstAuditLogs.Count > 0)
                {
                    DataRow dr = null;
                    int rowDisplayNum = 1;
                    StringBuilder sbItemInfo = new StringBuilder(string.Empty);
                    StringBuilder sbItemContent = new StringBuilder(string.Empty);

                    foreach (AuditLog_Item log in lstAuditLogs)
                    {
                        sbItemInfo = new StringBuilder(string.Empty);
                        sbItemContent = new StringBuilder(string.Empty);

                        sbItemInfo.AppendLine("<strong>ID: </strong>" + log.ItemId + "<br>");
                        sbItemInfo.AppendLine("<strong>Name: </strong>" + log.ItemName + "<br>");
                        sbItemInfo.AppendLine("<strong>Path: </strong>" + log.ItemPath + "<br>");
                        sbItemInfo.AppendLine("<strong>Template ID: </strong>" + log.TemplateId + "<br>");
                        sbItemInfo.AppendLine("<strong>Language: </strong>" + log.ItemLanguage);
                        sbItemInfo.AppendLine("<strong>Version: </strong>" + log.ItemVersion);

                        sbItemContent.Append("<button type=\"button\" data-rowid=\"" + log.RowId + "\" class=\"btn btn-warning btn-sm btnViewItemAuditLog\">View</button>");
                        sbItemContent.Append(string.Format("<div class=\"hidden divIdbs-{0}\">{1}</div>", log.RowId, log.ItemDataBeforeSave));
                        sbItemContent.Append(string.Format("<div class=\"hidden divIdas-{0}\">{1}</div>", log.RowId, log.ItemDataAfterSave));
                        HtmlDiff.HtmlDiff diff = new HtmlDiff.HtmlDiff(log.ItemDataBeforeSave, log.ItemDataAfterSave);
                        sbItemContent.Append(string.Format("<div class=\"hidden divIdDiff-{0}\">{1}</div>", log.RowId, diff.Build()));

                        dr = dtLogs.NewRow();
                        dr[Constants.ColumnNames.Row_Id] = rowDisplayNum;
                        dr[Constants.ColumnNames.Item_Info] = sbItemInfo.ToString();
                        dr[Constants.ColumnNames.Event] = log.Event;
                        dr[Constants.ColumnNames.Actioned_By] = log.ActionedBy;
                        dr[Constants.ColumnNames.Comments] = log.Comments;
                        dr[Constants.ColumnNames.Logged_Time] = log.LoggedTime.ToString("dd-MMM-yyyy HH:mm:ss.fff");
                        dr[Constants.ColumnNames.Item_Content] = sbItemContent.ToString();
                        dtLogs.Rows.Add(dr);
                        rowDisplayNum++;
                    }
                }
            }

            return dtLogs;
        }

        private static DataTable GetAuditLogsTableSchema()
        {
            DataTable dtLogs = new DataTable();
            dtLogs.TableName = "dtIal";
            dtLogs.Columns.Add(Constants.ColumnNames.Row_Id);
            dtLogs.Columns.Add(Constants.ColumnNames.Item_Info);
            dtLogs.Columns.Add(Constants.ColumnNames.Event);
            dtLogs.Columns.Add(Constants.ColumnNames.Actioned_By);
            dtLogs.Columns.Add(Constants.ColumnNames.Comments);
            dtLogs.Columns.Add(Constants.ColumnNames.Logged_Time);
            dtLogs.Columns.Add(Constants.ColumnNames.Item_Content);            
            return dtLogs;
        }

        
    }
}
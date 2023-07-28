namespace Basiscore.CmsAudit.sitecore.admin.cms_audit
{
    using Basiscore.CmsAudit.Models;
    using Basiscore.CmsAudit.Services;
    using Basiscore.CmsAudit.Utilities;
    using Sitecore.Diagnostics;
    using System;
    using System.Data;
    using System.Web.Script.Serialization;
    using System.Web.Services;

    public partial class purge_logs : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (SitecoreUtility.IsAdministrator())
                {
                    if (!IsPostBack)
                    {
                        BindItemAuditLogsSummary();
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
            }
        }

        private void BindItemAuditLogsSummary()
        {
            DataTable dt = new DbService().GetItemAuditDataSummary();

            if (dt != null && dt.Rows.Count > 0)
            {
                ltFirstRecordDate_Ial.Text = Convert.ToDateTime(Convert.ToString(dt.Rows[0][Constants.ColumnNames.FirstLogDate])).ToString("dd-MMM-yyyy HH:mm:ss.fff");
                ltRecentRecordDate_Ial.Text = Convert.ToDateTime(Convert.ToString(dt.Rows[0][Constants.ColumnNames.RecentLogDate])).ToString("dd-MMM-yyyy HH:mm:ss.fff");
                ltTotalRecords_Ial.Text = Convert.ToString(dt.Rows[0][Constants.ColumnNames.TotalRows]);
            }
        }

        [WebMethod]
        public static string PurgeItemAuditLogs(DateTime fromDate, DateTime toDate)
        {
            string output = "";
            BaseResponse result = new BaseResponse();

            try
            {
                if (SitecoreUtility.IsAdministrator())
                {
                    int deletedRows = new DbService().DeleteItemAuditLogs(fromDate, toDate);
                    result.StatusMessage = Convert.ToString(deletedRows);
                    result.StatusCode = 1;
                }
                else
                {
                    result.StatusCode = 2;
                }
            }
            catch (Exception ex)
            {
                Log.Error(Constants.ModuleName, ex, Constants.ModuleName);
                result.StatusCode = 0;
                result.StatusMessage = Constants.GenericErrorMessage;
                result.ErrorMessage = ex.Message;
            }

            output = new JavaScriptSerializer().Serialize(result);
            return output;
        }
    }
}
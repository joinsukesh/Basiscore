

namespace Basiscore.Minions.sitecore.admin.minions
{
    using Basiscore.Minions.Utilities;
    using System;
    using System.Data;
    using System.Web;

    public partial class download : System.Web.UI.Page
    {
        #region EVENTS

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                string data = Request.QueryString["data"];
                string id = Request.QueryString["id"];

                if (!string.IsNullOrEmpty(data) && !string.IsNullOrEmpty(id))
                {
                    switch (data)
                    {
                        case "renderingusage":
                            DownloadRenderingUsageData(id);
                            break;
                        case "finditems":
                            DownloadFindItemsData(id);
                            break;
                        case "fieldvalues":
                            DownloadFieldValuesData(id);
                            break;
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        #endregion

        #region METHODS

        private void DownloadRenderingUsageData(string sessionId)
        {
            if (HttpContext.Current.Session[sessionId] != null)
            {
                DataTable dt = (DataTable)HttpContext.Current.Session[sessionId];

                if (dt != null && dt.Rows.Count > 0)
                {
                    if (dt.Columns.Contains("PageItemPath"))
                    {
                        dt.Columns["PageItemPath"].ColumnName = "ITEM_PATH"; 
                    }

                    if (dt.Columns.Contains("StatusMessage"))
                    {
                        dt.Columns["StatusMessage"].ColumnName = "TARGET_LAYOUTS";
                    }

                    if (dt.Columns.Contains("StatusCode"))
                    {
                        dt.Columns.Remove("StatusCode");
                    }

                    if (dt.Columns.Contains("TargetLanguages"))
                    {
                        dt.Columns.Remove("TargetLanguages");
                    }

                    dt.AcceptChanges();
                    MinionHelper.DownloadData(dt, "RenderingUsage_" + DateTime.Now.ToString("ddMMyyyyHHmmss"));
                }
            }
        }

        private void DownloadFindItemsData(string sessionId)
        {
            if (HttpContext.Current.Session[sessionId] != null)
            {
                DataTable dt = (DataTable)HttpContext.Current.Session[sessionId];

                if (dt != null && dt.Rows.Count > 0)
                {
                    if (dt.Columns.Contains("Fields"))
                    {
                        dt.Columns.Remove("Fields");
                    }

                    if (dt.Columns.Contains("MatchLog"))
                    {
                        dt.Columns.Remove("MatchLog");
                    }

                    if (dt.Columns.Contains("FieldNames"))
                    {
                        dt.Columns["FieldNames"].ColumnName = "Fields";
                    }

                    dt.AcceptChanges();
                    MinionHelper.DownloadData(dt, "FindItems_" + DateTime.Now.ToString("ddMMyyyyHHmmss"));
                }
            }
        }

        private void DownloadFieldValuesData(string sessionId)
        {
            if (HttpContext.Current.Session[sessionId] != null)
            {
                DataTable dt = (DataTable)HttpContext.Current.Session[sessionId];

                if (dt != null && dt.Rows.Count > 0)
                {
                    if (dt.Columns.Contains("Fields"))
                    {
                        dt.Columns.Remove("Fields");
                    }

                    if (dt.Columns.Contains("MatchLog"))
                    {
                        dt.Columns["MatchLog"].ColumnName = "FieldValue";
                    }

                    if (dt.Columns.Contains("FieldNames"))
                    {
                        dt.Columns["FieldNames"].ColumnName = "Field";
                    }

                    dt.AcceptChanges();
                    MinionHelper.DownloadData(dt, "FieldValues_" + DateTime.Now.ToString("ddMMyyyyHHmmss"));
                }
            }
        }

        #endregion
    }
}
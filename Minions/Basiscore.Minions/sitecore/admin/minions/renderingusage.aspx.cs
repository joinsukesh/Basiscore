
namespace Basiscore.Minions.sitecore.admin.minions
{
    using Basiscore.Minions;
    using Basiscore.Minions.Models;
    using Basiscore.Minions.Utilities;
    using Sitecore.Data;
    using Sitecore.Data.Items;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Web;
    using System.Web.Script.Serialization;
    using System.Web.Services;
    using System.Web.UI;

    public partial class renderingusage : System.Web.UI.Page
    {
        #region EVENTS

        protected void Page_Load(object sender, EventArgs e)
        {
            if (MinionHelper.IsUserLoggedIn())
            {
                try
                {
                    BindData();
                    hdnSessionId.Value = MinionHelper.GetRandomString(7);
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
        public static string GetRenderingUsageResult(RenderingsModuleDataModel dataModel)
        {
            RenderingModuleResult result = new RenderingModuleResult();
            string error = "";
            string output = "";
            bool hasFailedItems = false;

            try
            {
                if (MinionHelper.IsUserLoggedIn())
                {
                    if (IsValidModel(dataModel, out error))
                    {
                        List<Item> pageItems = GetTargetPageItems(dataModel);
                        result.LstRenderingStatus = FindRenderingInPages(dataModel, pageItems, out hasFailedItems, out error);

                        if (hasFailedItems)
                        {
                            if (result.LstRenderingStatus == null || result.LstRenderingStatus.Count <= 0)
                            {
                                result.StatusCode = 0;
                                result.StatusMessage = "Could not process the task. " + error;
                            }
                            else
                            {
                                result.StatusCode = 0;
                                result.StatusMessage = "Could not find rendering for one or more items. Check the table for errors.";
                            }
                        }
                        else
                        {
                            if (result.LstRenderingStatus == null || result.LstRenderingStatus.Count <= 0)
                            {
                                result.StatusCode = 0;
                                result.StatusMessage = "Rendering not available in any of the page items, for given inputs.";
                            }
                            else
                            {
                                SaveResultInSession(dataModel.SessionId, result.LstRenderingStatus);
                                result.StatusCode = 1;
                                result.StatusMessage = "The rendering is available in the following pages";
                            }
                        }
                    }
                    else
                    {
                        result.StatusCode = 0;
                        result.Error = error;
                    }
                }
                else
                {
                    result.StatusCode = 2;
                }
            }
            catch (Exception ex)
            {
                result.LstRenderingStatus = new List<RenderingTaskStatus>();
                result.StatusCode = 0;
                result.Error = "Error: " + ex.Message;
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

        private static bool IsValidModel(RenderingsModuleDataModel dataModel, out string error)
        {
            error = string.Empty;

            if (dataModel != null)
            {
                if (dataModel.TaskId < 0 || dataModel.TaskId > 2)
                {
                    error = "Invalid Task; ";
                }

                if (dataModel.TaskId == 1)
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

                if (dataModel.TaskId == 2)
                {
                    List<string> lstItemPaths = dataModel.GetItemPaths();

                    if (lstItemPaths == null || lstItemPaths.Count == 0)
                    {
                        error += "Invalid item paths; ";
                    }
                }

                if (string.IsNullOrEmpty(dataModel.RenderingId) || !MinionHelper.IsValidID(dataModel.RenderingId))
                {
                    error += "Invalid Rendering Id; ";
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

        private static List<Item> GetTargetPageItems(RenderingsModuleDataModel dataModel)
        {
            List<Item> pageItems = null;

            if (dataModel.TaskId == 1)
            {
                Item parentItem = MinionHelper.GetItem(dataModel.ParentItemId, dataModel.SelectedLanguage, dataModel.DatabaseName);

                if (parentItem != null && parentItem.HasChildren)
                {
                    if (!string.IsNullOrEmpty(dataModel.TargetTemplateId))
                    {
                        pageItems = MinionHelper.GetItemsByTemplate(parentItem, new ID(dataModel.TargetTemplateId));
                    }
                    else
                    {
                        pageItems = parentItem.Axes.GetDescendants().ToList();
                    }
                }
            }
            else if (dataModel.TaskId == 2)
            {
                List<string> strItemPaths = dataModel.GetItemPaths();

                if (strItemPaths != null && strItemPaths.Count > 0)
                {
                    Item page = null;
                    pageItems = new List<Item>();

                    foreach (string itemPath in strItemPaths)
                    {
                        page = MinionHelper.GetItem(itemPath, dataModel.SelectedLanguage, dataModel.DatabaseName);

                        if (page != null)
                        {
                            pageItems.Add(page);
                        }
                    }
                }
            }

            return pageItems;
        }

        private static List<RenderingTaskStatus> FindRenderingInPages(RenderingsModuleDataModel dataModel, List<Item> pageItems, out bool hasFailedItems, out string taskStatusMessage)
        {
            hasFailedItems = false;
            List<RenderingTaskStatus> lstResults = null;
            taskStatusMessage = string.Empty;

            if (pageItems != null && pageItems.Count > 0)
            {
                string renderingId = dataModel.RenderingId;
                lstResults = new List<RenderingTaskStatus>();
                bool isInSharedLayout = false;
                bool isInFinalLayout = false;
                string status = string.Empty;

                foreach (Item page in pageItems)
                {
                    try
                    {
                        isInSharedLayout = false;
                        isInFinalLayout = false;
                        status = string.Empty;
                        GetRenderingUsageInfo(page, renderingId, out isInSharedLayout, out isInFinalLayout);
                    }
                    catch (Exception ex)
                    {
                        hasFailedItems = true;
                        status = "ERROR - " + ex.Message + "<br>";
                    }

                    if (isInSharedLayout || isInFinalLayout)
                    {
                        status = isInSharedLayout ? "Shared, " : "";
                        status += isInFinalLayout ? "Final" : "";
                        status = status.Trim().Trim(',');

                        lstResults.Add(new RenderingTaskStatus
                        {
                            PageItemPath = page.Paths.FullPath,
                            StatusCode = 1,
                            StatusMessage = status
                        });
                    }
                }
            }
            else
            {
                hasFailedItems = true;
                taskStatusMessage = "No page items found for the given inputs";
            }

            return lstResults;
        }

        /// <summary>
        /// get the details of a rendering like, if it is used in the page, used in shared layout and or final layout.
        /// </summary>
        /// <param name="contextItem"></param>
        /// <param name="renderingID"></param>
        /// <returns></returns>
        private static void GetRenderingUsageInfo(Item contextItem, string renderingID, out bool isInSharedLayout, out bool isInFinalLayout)
        {
            isInSharedLayout = false;
            isInFinalLayout = false;

            if (contextItem != null && contextItem.Versions.Count > 0)
            {
                if (contextItem.Fields[MinionConstants.Templates.Layout.Fields.__Renderings].Value.Contains(renderingID))
                {
                    isInSharedLayout = true;
                }

                if (contextItem.Fields[MinionConstants.Templates.Layout.Fields.__FinalRenderings].Value.Contains(renderingID))
                {
                    isInFinalLayout = true;
                }
            }
        }

        private static void SaveResultInSession(string sessionId, List<RenderingTaskStatus> data)
        {
            HttpContext.Current.Session[sessionId] = null;

            try
            {
                if (data != null && data.Count > 0)
                {
                    HttpContext.Current.Session[sessionId] = data.ToDataTable<RenderingTaskStatus>();
                }
            }
            catch (Exception ex)
            {

            }
        }

        #endregion
    }
}
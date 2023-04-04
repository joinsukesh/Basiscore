
namespace Basiscore.Minions.sitecore.admin.minions
{
    using Basiscore.Minions;
    using Basiscore.Minions.Models;
    using Basiscore.Minions.Utilities;
    using Sitecore;
    using Sitecore.Data;
    using Sitecore.Data.Fields;
    using Sitecore.Data.Items;
    using Sitecore.Globalization;
    using Sitecore.Layouts;
    using Sitecore.SecurityModel;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Script.Serialization;
    using System.Web.Services;

    public partial class updatedatasource : System.Web.UI.Page
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
        public static string UpdateDatasourceForRenderings(RenderingsModuleDataModel dataModel)
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
                        result.LstRenderingStatus = UpdateRenderingDatasourceInPages(dataModel, pageItems, out hasFailedItems, out error);

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
                                result.StatusMessage = "Could not update datasource for one or more items. Check the table for errors.";
                            }
                        }
                        else
                        {
                            result.StatusCode = 1;

                            if (result.LstRenderingStatus == null || result.LstRenderingStatus.Count <= 0)
                            {
                                result.StatusMessage = "No datasources updated. The rendering may be unavailable for the given inputs.";
                            }
                            else
                            {
                                result.StatusMessage = "The datasource is updated for the following pages";
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
                if (dataModel.TaskId < 0 || dataModel.TaskId > 4)
                {
                    error = "Invalid Task; ";
                }

                if (dataModel.TaskId == 3)
                {
                    if (dataModel.RenderingIndex <= -1)
                    {
                        error += "Invalid Rendering Index; ";
                    }
                }

                if (dataModel.TargetItemsTypeId == 1)
                {
                    if (string.IsNullOrEmpty(dataModel.ParentItemId) || !MinionHelper.IsValidID(dataModel.ParentItemId))
                    {
                        error += "Invalid Parent Item Id; ";
                    }

                    if (!string.IsNullOrEmpty(dataModel.TargetTemplateId) && !MinionHelper.IsValidID(dataModel.TargetTemplateId))
                    {
                        error += "Invalid target template Id; ";
                    }
                }
                else if (dataModel.TargetItemsTypeId == 2)
                {
                    List<string> lstItemPaths = dataModel.GetItemPaths();

                    if (lstItemPaths == null || lstItemPaths.Count == 0)
                    {
                        error += "Invalid item paths; ";
                    }
                }

                if (string.IsNullOrEmpty(dataModel.RenderingId) || !MinionHelper.IsValidID(dataModel.RenderingId))
                {
                    error += "Invalid rendering Id; ";
                }

                if (string.IsNullOrEmpty(dataModel.DatasourceId) || !MinionHelper.IsValidID(dataModel.DatasourceId))
                {
                    error += "Invalid datasource Id; ";
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

            if (dataModel.TargetItemsTypeId == 1)
            {
                Item parentItem = MinionHelper.GetItem(dataModel.ParentItemId);

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
            else if (dataModel.TargetItemsTypeId == 2)
            {
                List<string> strItemPaths = dataModel.GetItemPaths();

                if (strItemPaths != null && strItemPaths.Count > 0)
                {
                    Item page = null;
                    pageItems = new List<Item>();

                    foreach (string itemPath in strItemPaths)
                    {
                        page = MinionHelper.GetItem(itemPath);

                        if (page != null)
                        {
                            pageItems.Add(page);
                        }
                    }
                }
            }

            return pageItems;
        }

        private static List<RenderingTaskStatus> UpdateRenderingDatasourceInPages(RenderingsModuleDataModel dataModel, List<Item> pageItems, out bool hasFailedItems, out string taskStatusMessage)
        {
            hasFailedItems = false;
            List<RenderingTaskStatus> lstResults = null;
            taskStatusMessage = string.Empty;
            string errorMsg;

            if (pageItems != null && pageItems.Count > 0)
            {
                lstResults = new List<RenderingTaskStatus>();
                Language language = dataModel.SelectedLanguage;
                Item itemByLanguage = null;
                string languageName = string.Empty;
                string status = string.Empty;
                bool isDatasourceUpdated = false;
                //string errorMsg = string.Empty;

                foreach (Item page in pageItems)
                {
                    try
                    {
                        itemByLanguage = MinionHelper.GetItem(page.ID, language);
                        languageName = itemByLanguage.Language.Name;
                        status = string.Empty;
                        isDatasourceUpdated = false;
                        hasFailedItems = false;
                        errorMsg = string.Empty;

                        ///if this language has 0 versions, then no action required.
                        if (itemByLanguage.Versions.Count <= 0)
                        {
                            ///do nothing
                            status = languageName + " - Item has no versions.<br>";
                        }
                        else
                        {
                            isDatasourceUpdated = UpdateRenderingDatasource(itemByLanguage, dataModel, out errorMsg);

                            if (isDatasourceUpdated)
                            {
                                status += languageName + " - Rendering datasource updated.<br>";
                            }
                            else if (!string.IsNullOrEmpty(errorMsg))
                            {
                                status += languageName + " - " + errorMsg + "<br>";
                                hasFailedItems = true;
                                isDatasourceUpdated = false;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        hasFailedItems = true;
                        isDatasourceUpdated = false;
                        status = languageName + " - " + ex.Message + "<br>";
                    }

                    if (isDatasourceUpdated || hasFailedItems)
                    {
                        lstResults.Add(new RenderingTaskStatus
                        {
                            PageItemPath = page.Paths.FullPath,
                            TargetLanguages = languageName,
                            StatusCode = isDatasourceUpdated ? 1 : 0,
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

        private static bool UpdateRenderingDatasource(Item targetItem, RenderingsModuleDataModel dataModel, out string errorMsg)
        {
            errorMsg = string.Empty;
            Item itemToUpdate = null;
            bool isDatasourceModified = false;
            RenderingDefinition instanceOfRendering = null;
            int taskId = dataModel.TaskId;
            string renderingId = dataModel.RenderingId;
            int renderingIndex = dataModel.RenderingIndex;
            bool createVersion = dataModel.CreateVersion;
            bool isForFinalLayout = dataModel.TargetLayoutId == 2;
            string defaultDeviceId = MinionConstants.Items.DefaultLayoutDeviceId;
            LayoutField layoutField = null;
            LayoutDefinition layoutDefinition = null;
            DeviceDefinition deviceDefinition = null;
            MinionHelper.GetDeviceDefinitions(targetItem, isForFinalLayout, defaultDeviceId, out layoutField, out layoutDefinition, out deviceDefinition);


            if (deviceDefinition != null)
            {
                /// Get the array of all renderings for the target page item	                
                IEnumerable<RenderingDefinition> renderingsArray = deviceDefinition.Renderings.ToArray().Cast<RenderingDefinition>();

                if (renderingsArray.Count() > 0 && MinionHelper.PageHasRendering(renderingId, renderingsArray, taskId == 3, renderingIndex))
                {
                    if (createVersion)
                    {
                        itemToUpdate = targetItem.Versions.AddVersion();
                        MinionHelper.GetDeviceDefinitions(itemToUpdate, isForFinalLayout, defaultDeviceId, out layoutField, out layoutDefinition, out deviceDefinition);
                    }
                    else
                    {
                        itemToUpdate = targetItem;
                    }

                    switch (dataModel.TaskId)
                    {
                        case 1:  /// Modify datasource for first instance of rendering                        

                            instanceOfRendering = MinionHelper.GetRenderingDefinition(dataModel.RenderingId, renderingsArray, MinionHelper.RenderingInstancePosition.First, dataModel.RenderingIndex);

                            if (instanceOfRendering != null)
                            {
                                instanceOfRendering.Datasource = dataModel.DatasourceId;
                                isDatasourceModified = true;
                            }

                            break;
                        case 2:  /// Modify datasource for last instance of rendering

                            instanceOfRendering = MinionHelper.GetRenderingDefinition(dataModel.RenderingId, renderingsArray, MinionHelper.RenderingInstancePosition.Last, dataModel.RenderingIndex);

                            if (instanceOfRendering != null)
                            {
                                instanceOfRendering.Datasource = dataModel.DatasourceId;
                                isDatasourceModified = true;
                            }

                            break;
                        case 3:  /// Modify datasource of rendering at specified index
                            instanceOfRendering = MinionHelper.GetRenderingDefinition(dataModel.RenderingId, renderingsArray, MinionHelper.RenderingInstancePosition.SpecifiedIndex, dataModel.RenderingIndex);

                            if (instanceOfRendering != null && !string.IsNullOrEmpty(instanceOfRendering.UniqueId))
                            {
                                instanceOfRendering.Datasource = dataModel.DatasourceId;
                                isDatasourceModified = true;
                            }

                            break;
                        case 4:  /// Modify datasource for all instances of rendering
                            RenderingDefinition rdefn = null;
                            foreach (object obj in deviceDefinition.Renderings)
                            {
                                rdefn = (RenderingDefinition)obj;

                                if (rdefn != null && rdefn.ItemID == dataModel.RenderingId)
                                {
                                    rdefn.Datasource = dataModel.DatasourceId;
                                    isDatasourceModified = true;
                                }
                            }
                            break;
                    }

                    /// Save the layout changes
                    using (new SecurityDisabler())
                    {
                        try
                        {
                            itemToUpdate.Editing.BeginEdit();
                            layoutField.Value = layoutDefinition.ToXml();
                            itemToUpdate.Editing.EndEdit();
                        }
                        catch (Exception ex)
                        {
                            itemToUpdate.Editing.CancelEdit();
                            isDatasourceModified = false;
                        }

                        ///copy final layout renderings to shared layout
                        if (isForFinalLayout && dataModel.CopyFinalRenderingsToShared)
                        {
                            ///If we don't have a final layout delta, we're good!
                            if (!string.IsNullOrEmpty(layoutField.Value))
                            {
                                using (new EditContext(itemToUpdate))
                                {
                                    try
                                    {
                                        LayoutField sharedLayoutField = new LayoutField(itemToUpdate.Fields[FieldIDs.LayoutField]);
                                        sharedLayoutField.Value = layoutDefinition.ToXml();
                                        itemToUpdate.Fields[MinionConstants.FieldNames.FinalRenderings].Reset();
                                        itemToUpdate.Editing.AcceptChanges();
                                    }
                                    catch (Exception ex)
                                    {
                                        itemToUpdate.Editing.CancelEdit();
                                        errorMsg = "Could not copy final layout renderings to shared layout.";
                                    }
                                }
                            }
                        }
                    }

                    if (!isDatasourceModified)
                    {
                        errorMsg = "Could not update the datasource.";
                    }
                }
            }

            return isDatasourceModified;
        }

        #endregion
    }
}

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
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Script.Serialization;
    using System.Web.Services;

    public partial class addrendering : System.Web.UI.Page
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
        public static string AddRendering(RenderingsModuleDataModel dataModel)
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
                        result.LstRenderingStatus = AddRenderingToPages(dataModel, pageItems, out hasFailedItems, out error);

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
                                result.StatusMessage = "Could not add the rendering for one or more items. Check the table for errors.";
                            }
                        }
                        else
                        {
                            result.StatusCode = 1;
                            result.StatusMessage = "The rendering is added to the following pages";
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

                if (!string.IsNullOrEmpty(dataModel.DatasourceId) && !MinionHelper.IsValidID(dataModel.DatasourceId))
                {
                    error += "Invalid Datasource Id; ";
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
            else if (dataModel.TaskId == 2)
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

        private static List<RenderingTaskStatus> AddRenderingToPages(RenderingsModuleDataModel dataModel, List<Item> pageItems, out bool hasFailedItems, out string taskStatusMessage)
        {
            hasFailedItems = false;
            List<RenderingTaskStatus> lstResults = null;
            taskStatusMessage = string.Empty;

            if (pageItems != null && pageItems.Count > 0)
            {
                lstResults = new List<RenderingTaskStatus>();
                Language language = dataModel.SelectedLanguage;
                Item itemByLanguage = null;
                string languageName = string.Empty;
                Item itemToUpdate = null;
                string status = string.Empty;
                bool isRenderingAdded = false;

                foreach (Item page in pageItems)
                {
                    try
                    {
                        itemToUpdate = null;
                        itemByLanguage = MinionHelper.GetItem(page.ID, language);
                        languageName = itemByLanguage.Language.Name;
                        status = string.Empty;
                        isRenderingAdded = false;                        

                        ///if this language has 0 versions and option to create version is not checked, then do not add rendering.
                        if (!dataModel.CreateVersion && itemByLanguage.Versions.Count <= 0)
                        {
                            ///do nothing
                            hasFailedItems = true;
                            status = languageName + " - Item has no versions.<br>";
                        }
                        else
                        {
                            itemToUpdate = dataModel.CreateVersion ? itemByLanguage.Versions.AddVersion() : itemByLanguage;
                            AddRendering(itemToUpdate, dataModel.RenderingId, dataModel.Placeholder, dataModel.DatasourceId, dataModel.RenderingIndex);
                            isRenderingAdded = true;
                            status += languageName + " - Rendering added.<br>";
                        }
                    }
                    catch (Exception ex)
                    {
                        hasFailedItems = true;
                        isRenderingAdded = false;
                        status = languageName + " - " + ex.Message + "<br>";
                    }

                    lstResults.Add(new RenderingTaskStatus
                    {
                        PageItemPath = page.Paths.FullPath,
                        TargetLanguages = languageName,
                        StatusCode = isRenderingAdded ? 1 : 0,
                        StatusMessage = status
                    });
                }
            }
            else
            {
                hasFailedItems = true;
                taskStatusMessage = "No page items found for the given inputs";
            }

            return lstResults;
        }

        private static void AddRendering(Item pageItem, string renderingId, string placeholder, string datasourceId, int renderingIndex)
        {
            if (pageItem != null)
            {
                /// Get the layout definitions and the device definition	                
                LayoutField layoutField = new LayoutField(pageItem.Fields[FieldIDs.LayoutField]);
                LayoutDefinition layoutDefinition = LayoutDefinition.Parse(layoutField.Value);

                /// /sitecore/layout/Devices/Default
                string defaultDeviceId = MinionConstants.Items.DefaultLayoutDeviceId;

                DeviceDefinition deviceDefinition = layoutDefinition.GetDevice(defaultDeviceId);
                DeviceItem deviceItem = new DeviceItem(MinionHelper.GetItem(defaultDeviceId));

                if (deviceDefinition != null && deviceItem != null)
                {
                    /// Create a RenderingDefinition and add the reference of sublayout or rendering	                
                    RenderingDefinition renderingDefinition = new RenderingDefinition();
                    renderingDefinition.ItemID = renderingId;

                    /// Set placeholder where the rendering should be displayed	                
                    renderingDefinition.Placeholder = placeholder;

                    /// Set the datasource of sublayout, if any	                
                    renderingDefinition.Datasource = datasourceId;

                    /// Get all added renderings	                
                    RenderingReference[] renderings = pageItem.Visualization.GetRenderings(deviceItem, true);

                    if (renderingIndex < 0 || renderings == null || renderings.Length <= 0 || renderingIndex >= renderings.Length)
                    {
                        /// Add rendering to end of list	                    
                        deviceDefinition.AddRendering(renderingDefinition);
                    }
                    else
                    {
                        /// Add rendering at specified index	                    
                        deviceDefinition.Insert(renderingIndex, renderingDefinition);
                    }

                    /// Save the layout changes	                
                    using (new SecurityDisabler())
                    {
                        pageItem.Editing.BeginEdit();
                        layoutField.Value = layoutDefinition.ToXml();
                        pageItem.Editing.EndEdit();
                    } 
                }                
            }
        }

        #endregion
    }
}


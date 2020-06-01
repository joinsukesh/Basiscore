
namespace Basiscore.Minions.sitecore.admin.minions
{
    using Sitecore.Data;
    using Sitecore.Data.Items;
    using Sitecore.Globalization;
    using Sitecore.Publishing.Pipelines.Publish;
    using Basiscore.Minions.Models;
    using Basiscore.Minions.Utilities;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.IO.Compression;
    using System.Linq;
    using System.Text;
    using System.Web.Script.Serialization;
    using System.Web.Services;
    using System.Xml;

    public partial class publishitems : System.Web.UI.Page
    {
        #region VARIABLES

        public struct ErrorMessages
        {
            public static string SOURCE_NOT_FOUND = "Source item not found";
            public static string COULD_NOT_PUBLISH = "Could not publish";
            public static string NEVER_PUBLISH = "Item set to never publish";
        }

        #endregion

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

        #endregion

        #region METHODS

        private void BindData()
        {
            chkLanguages.DataSource = MinionHelper.GetInstalledLanguages();
            chkLanguages.DataTextField = "Value";
            chkLanguages.DataValueField = "Key";
            chkLanguages.DataBind();

            chkDatabases.DataSource = MinionHelper.GetPublishingTargets();
            chkDatabases.DataTextField = "Key";
            chkDatabases.DataValueField = "Value";
            chkDatabases.DataBind();
        }

        [WebMethod]
        public static string PublishSitecoreItems(CustomPublishDataModel customPublishDataModel)
        {
            CustomPublishReport result = new CustomPublishReport();
            string error = "";
            string output = "";

            try
            {
                if (MinionHelper.IsUserLoggedIn())
                {
                    if (IsValidModel(customPublishDataModel))
                    {
                        List<ItemPublishedStatus> lstItemPublishStatus = PublishUserInputItems(customPublishDataModel, out error);
                        result.LstItemPublishStatus = lstItemPublishStatus != null ? lstItemPublishStatus : new List<ItemPublishedStatus>();
                        result.Error = error;
                        result.PublishStatus = 1;
                    }
                }
                else
                {
                    result.PublishStatus = 2;
                }
            }
            catch (Exception ex)
            {
                result.LstItemPublishStatus = new List<ItemPublishedStatus>();
                result.PublishStatus = 0;
                result.Error = ex.Message;
            }

            output = new JavaScriptSerializer().Serialize(result);
            return output;
        }

        [WebMethod]
        public static string GetPackageItemPaths(string packagePath)
        {
            CustomPublishReport result = new CustomPublishReport();
            string error = "";
            string output = "";

            try
            {
                if (MinionHelper.IsUserLoggedIn())
                {
                    if (!string.IsNullOrEmpty(packagePath))
                    {
                        List<string> lstItemPaths = GetMasterDbItemPathsFromPackage(packagePath, out error);
                        result.LstPackageItemPaths = lstItemPaths != null ? lstItemPaths : new List<string>();
                        result.Error = error;
                        result.GetItemPathsStatus = 1;
                    }
                }
                else
                {
                    result.GetItemPathsStatus = 2;
                }
            }
            catch (Exception ex)
            {
                result.LstPackageItemPaths = new List<string>();
                result.GetItemPathsStatus = 0;
                result.Error = ex.Message;
            }

            output = new JavaScriptSerializer().Serialize(result);
            return output;
        }

        /// <summary>
        /// For the model to be valid, it should:
        /// 1. have the item paths list from atleast one of the textboxes
        /// 2. have atleast one target language
        /// 3. have atleast one target database
        /// </summary>
        /// <param name="customPublishDataModel"></param>
        /// <returns></returns>
        private static bool IsValidModel(CustomPublishDataModel customPublishDataModel)
        {
            bool isValidModel = false;
            List<string> lstItemPathsToPublishWithSubitems = customPublishDataModel != null ? customPublishDataModel.GetItemPathsToPublishWithSubitems() : new List<string>();
            List<string> lstItemPathsToPublish = customPublishDataModel != null ? customPublishDataModel.GetItemPathsToPublish() : new List<string>();

            if ((lstItemPathsToPublishWithSubitems.Count > 0 || lstItemPathsToPublish.Count > 0) &&
                customPublishDataModel.SelectedDatabaseNames.Count > 0 && customPublishDataModel.SelectedLanguages.Count > 0)
            {
                isValidModel = true;
            }

            return isValidModel;
        }

        /// <summary>
        /// read the user give list of items paths & publish them
        /// </summary>
        /// <param name="customPublishDataModel"></param>
        /// <param name="errorLog"></param>
        /// <returns></returns>
        private static List<ItemPublishedStatus> PublishUserInputItems(CustomPublishDataModel customPublishDataModel, out string errorLog)
        {
            errorLog = "";
            List<ItemPublishedStatus> lstItemPublishStatus = new List<ItemPublishedStatus>();
            List<ItemPublishedStatus> tempListOfPublishedItems = new List<ItemPublishedStatus>();
            List<ItemPublishedStatus> currentItemPublishStats = new List<ItemPublishedStatus>();
            StringBuilder sbErrorLog = new StringBuilder("");
            ItemPublishedStatus itemPublishStatus = null;
            int itemsCreated = 0;
            int itemsUpdated = 0;
            int itemsSkipped = 0;
            string itemsCreatedStatus = "";
            string itemsUpdatedStatus = "";
            string itemsSkippedStatus = "";

            try
            {
                List<Language> languages = customPublishDataModel.SelectedLanguages;
                List<string> databaseNames = customPublishDataModel.SelectedDatabaseNames;
                bool excludeItemsWithWorkflow = customPublishDataModel.ExcludeItemsWithWorkflow;
                List<string> lstItemPathsToPublishWithSubitems = customPublishDataModel.GetItemPathsToPublishWithSubitems();
                List<string> lstItemPathsToPublish = customPublishDataModel.GetItemPathsToPublish();
                List<string> allItemPaths = new List<string>();
                List<KeyValuePair<int, string>> lstPublishedItemStatsPerDb = new List<KeyValuePair<int, string>>();

                ///publish parent & children from the list given in the first textbox
                tempListOfPublishedItems = PublishWithSubItems(lstItemPathsToPublishWithSubitems, databaseNames, languages, excludeItemsWithWorkflow, out errorLog);

                if (!string.IsNullOrEmpty(errorLog))
                {
                    sbErrorLog.Append(errorLog);
                    errorLog = "";
                }

                if (tempListOfPublishedItems != null && tempListOfPublishedItems.Count > 0)
                {
                    lstItemPublishStatus.AddRange(tempListOfPublishedItems);
                }

                ///publish items from the list given in the second textbox
                if (lstItemPathsToPublish.Count > 0)
                {
                    tempListOfPublishedItems = null;
                    errorLog = "";
                    tempListOfPublishedItems = GetPublishedItemsStats(lstItemPathsToPublish, databaseNames, languages, excludeItemsWithWorkflow, out errorLog);

                    if (!string.IsNullOrEmpty(errorLog))
                    {
                        sbErrorLog.Append(errorLog);
                        errorLog = "";
                    }

                    if (tempListOfPublishedItems != null && tempListOfPublishedItems.Count > 0)
                    {
                        IEnumerable<string> uniqueItemPaths = tempListOfPublishedItems.Select(x => x.ItemPath).Distinct();

                        if (uniqueItemPaths != null && uniqueItemPaths.Count() > 0)
                        {
                            foreach (string itemPath in uniqueItemPaths)
                            {
                                itemPublishStatus = new ItemPublishedStatus();
                                itemPublishStatus.ItemPath = itemPath;

                                ///get the published stats for each item, in each db
                                foreach (string dbName in databaseNames)
                                {
                                    itemsCreated = tempListOfPublishedItems.Where(x => x.ItemPath == itemPath && x.TargetDatabase == dbName).Sum(x => x.ItemsCreated);
                                    itemsUpdated = tempListOfPublishedItems.Where(x => x.ItemPath == itemPath && x.TargetDatabase == dbName).Sum(x => x.ItemsUpdated);
                                    itemsSkipped = tempListOfPublishedItems.Where(x => x.ItemPath == itemPath && x.TargetDatabase == dbName).Sum(x => x.ItemsSkipped);

                                    GetItemPublishedStatsSummary(itemsCreated, itemsUpdated, itemsSkipped, dbName, out itemsCreatedStatus, out itemsUpdatedStatus, out itemsSkippedStatus);
                                    itemPublishStatus.ItemsCreatedStatus += itemsCreatedStatus;
                                    itemPublishStatus.ItemsUpdatedStatus += itemsUpdatedStatus;
                                    itemPublishStatus.ItemsSkippedStatus += itemsSkippedStatus;
                                }

                                lstItemPublishStatus.Add(itemPublishStatus);
                            }
                        }
                    }
                }
                else
                {

                }
            }
            catch (Exception ex)
            {
                sbErrorLog.Append(ex.Message);
            }

            errorLog = Convert.ToString(sbErrorLog);
            return lstItemPublishStatus;
        }

        /// <summary>
        /// build the html for the error logs
        /// </summary>
        /// <param name="itemPath"></param>
        /// <param name="language"></param>
        /// <param name="dbName"></param>
        /// <param name="ex"></param>
        /// <returns></returns>
        private static StringBuilder GetErrorLog(string itemPath, Language language, string dbName, Exception ex)
        {
            StringBuilder sb = new StringBuilder("");
            sb.AppendLine("<span class='errorLog'># " + ex.Message + "</span>");
            sb.AppendLine("<span><strong>Item Path: </strong>" + itemPath + "</span>");

            sb.AppendLine("<span>");

            if (language != null)
            {
                sb.AppendLine("<strong>Language: </strong>" + language.Name + "&nbsp;&nbsp;");
            }

            if (!string.IsNullOrEmpty(dbName))
            {
                sb.AppendLine("<strong>Target Database: </strong>" + dbName);
            }

            sb.AppendLine("</span>");

            sb.AppendLine("<br/><br/>");
            return sb;
        }

        /// <summary>
        /// for the give item paths, publish each item and collect the publish stats.
        /// return the collection
        /// </summary>
        /// <param name="itemPaths"></param>
        /// <param name="databaseNames"></param>
        /// <param name="languages"></param>
        /// <param name="excludeItemsWithWorkflow"></param>
        /// <param name="errorLog"></param>
        /// <returns></returns>
        private static List<ItemPublishedStatus> GetPublishedItemsStats(List<string> itemPaths, List<string> databaseNames,
            List<Language> languages, bool excludeItemsWithWorkflow, out string errorLog)
        {
            StringBuilder sbErrorLog = new StringBuilder("");
            ItemPublishedStatus itemPublishStatus = null;
            List<ItemPublishedStatus> lstItemPublishStatus = new List<ItemPublishedStatus>();
            PublishResult publishResult = null;
            Item sourceItem = null;
            bool isExcluded = false;
            string workflowId = string.Empty;
            bool hasWorkflow;

            if (itemPaths != null && itemPaths.Count > 0)
            {
                foreach (string itemPath in itemPaths)
                {
                    isExcluded = false;

                    foreach (string dbName in databaseNames)
                    {
                        itemPublishStatus = new ItemPublishedStatus();
                        itemPublishStatus.ItemPath = itemPath;
                        itemPublishStatus.TargetDatabase = dbName;

                        foreach (Language language in languages)
                        {
                            publishResult = null;

                            try
                            {
                                ///get the item for this language
                                sourceItem = MinionHelper.GetItem(itemPath, language);

                                ///proceed to publish if it is non-content or it has versions
                                if (sourceItem != null)
                                {
                                    if (sourceItem.Fields[MinionConstants.Templates.Publishing.Fields.NeverPublish].Value != MinionConstants.One)
                                    {
                                        hasWorkflow = MinionHelper.HasWorkflow(sourceItem);

                                        ///if excludeitemswithworkflow option is selected, check if item has workflow & exlcude if yes
                                        ///Also, if the item is non-content (like template, rendering, media etc), it SHOULD be published.
                                        if (excludeItemsWithWorkflow && itemPath.StartsWith(MinionConstants.Paths.Content))
                                        {
                                            if (hasWorkflow)
                                            {
                                                isExcluded = true;
                                                sourceItem = null;
                                            }
                                        }

                                        ///publish the item
                                        if (sourceItem != null)
                                        {
                                            if (hasWorkflow)
                                            {
                                                ///store the workflow id                                                
                                                workflowId = sourceItem.Fields[MinionConstants.Templates.Workflow.Fields.Workflow].Value;
                                                MinionHelper.RemoveWorkflow(sourceItem, language);
                                            }

                                            publishResult = MinionHelper.PublishItem(sourceItem, dbName, false);

                                            if (hasWorkflow)
                                            {
                                                ///if the item had workflow, then after the item is published, set the workflow id again & set the state to none.
                                                MinionHelper.UpdateFieldValues(sourceItem, new List<KeyValuePair<ID, string>> {
                                                    new KeyValuePair<ID, string> ( MinionConstants.Templates.Workflow.Fields.Workflow, workflowId ),
                                                    new KeyValuePair<ID, string> ( MinionConstants.Templates.Workflow.Fields.WorkflowState, string.Empty )
                                                }, false);
                                            }
                                        }

                                        ///add to error log only if this item is not excluded
                                        if (publishResult == null && !isExcluded)
                                        {
                                            sbErrorLog.Append(GetErrorLog(itemPath, language, dbName, new Exception(ErrorMessages.COULD_NOT_PUBLISH)));
                                        }
                                    }
                                    else
                                    {
                                        sbErrorLog.Append(GetErrorLog(itemPath, language, dbName, new Exception(ErrorMessages.NEVER_PUBLISH)));
                                    }
                                }
                                else
                                {
                                    sbErrorLog.Append(GetErrorLog(itemPath, language, dbName, new Exception(ErrorMessages.SOURCE_NOT_FOUND)));
                                }
                            }
                            catch (Exception ex)
                            {
                                sbErrorLog.Append(GetErrorLog(itemPath, language, dbName, ex));
                            }

                            ///increment the publish stats for this item,
                            ///so that each item will have stats per db
                            if (publishResult != null)
                            {
                                itemPublishStatus.ItemsCreated += publishResult.Statistics.Created;
                                itemPublishStatus.ItemsUpdated += publishResult.Statistics.Updated;
                                itemPublishStatus.ItemsSkipped += publishResult.Statistics.Skipped;
                            }
                        }

                        lstItemPublishStatus.Add(itemPublishStatus);
                    }
                }
            }

            errorLog = Convert.ToString(sbErrorLog);
            return lstItemPublishStatus;
        }

        /// <summary>
        /// read all master db items paths from sitecore package and return the list
        /// </summary>
        /// <param name="packageNameWithPath"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        private static List<string> GetMasterDbItemPathsFromPackage(string packageNameWithPath, out string error)
        {
            string targetFileName = "xml";
            List<string> lstItemPaths = new List<string>();
            error = "";

            using (FileStream fileStream = System.IO.File.Open(packageNameWithPath, FileMode.Open))
            {
                if (fileStream != null)
                {
                    ZipArchive parentArchive = new ZipArchive(fileStream);

                    if (parentArchive != null)
                    {
                        ///the default name of the inner zip file is package.zip
                        ZipArchiveEntry package = parentArchive.GetEntry("package.zip");

                        if (package != null)
                        {
                            ZipArchive childArchive = new ZipArchive(package.Open());

                            if (childArchive != null)
                            {
                                ///read only items that are from master db
                                IEnumerable<ZipArchiveEntry> xmls = childArchive.Entries
                                    .Where(x => x.FullName.StartsWith("items/master/") &&
                                    x.Name.ToLower() == targetFileName);

                                if (xmls != null && xmls.Count() > 0)
                                {
                                    XmlDocument doc = null;
                                    XmlElement root = null;
                                    string xmlText = "";
                                    string itemId = "";

                                    foreach (ZipArchiveEntry zae in xmls)
                                    {
                                        xmlText = new string((new StreamReader(
                                                             zae.Open(), Encoding.UTF8)
                                                             .ReadToEnd())
                                                             .ToArray());

                                        if (!string.IsNullOrEmpty(xmlText))
                                        {
                                            doc = new XmlDocument();
                                            doc.LoadXml(xmlText);
                                            root = doc.DocumentElement;

                                            if (root != null && root.Attributes["id"] != null)
                                            {
                                                itemId = root.Attributes["id"].Value;

                                                if (!string.IsNullOrEmpty(itemId))
                                                {
                                                    Item item = MinionHelper.Databases.masterDb.GetItem(itemId);

                                                    if (item != null && !lstItemPaths.Any(x => x == item.Paths.FullPath))
                                                    {
                                                        lstItemPaths.Add(item.Paths.FullPath);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    error = "No XML files found in the package";
                                }
                            }
                            else
                            {
                                error = "Could not read the sub directories";
                            }
                        }
                        else
                        {
                            error = "Could not read the package.zip file";
                        }
                    }
                    else
                    {
                        error = "Could not read the parent file";
                    }
                }
                else
                {
                    error = "Could not read the file";
                }
            }

            ///order the item paths by name
            if (lstItemPaths != null && lstItemPaths.Count > 0)
            {
                lstItemPaths = lstItemPaths.OrderBy(x => x).ToList();
            }

            return lstItemPaths;
        }

        /// <summary>
        /// build the summary of an item's published statistics to be displayed in the output table
        /// </summary>
        /// <param name="itemsCreated"></param>
        /// <param name="itemsUpdated"></param>
        /// <param name="itemsSkipped"></param>
        /// <param name="targetDbName"></param>
        /// <param name="itemsCreatedStatus"></param>
        /// <param name="itemsUpdatedStatus"></param>
        /// <param name="itemsSkippedStatus"></param>
        private static void GetItemPublishedStatsSummary(int itemsCreated, int itemsUpdated, int itemsSkipped, string targetDbName,
            out string itemsCreatedStatus, out string itemsUpdatedStatus, out string itemsSkippedStatus)
        {
            itemsCreatedStatus = itemsCreated + " for " + targetDbName + "<br/>";
            itemsUpdatedStatus = itemsUpdated + " for " + targetDbName + "<br/>";
            itemsSkippedStatus = itemsSkipped + " for " + targetDbName + "<br/>";
        }

        /// <summary>
        /// publish the parent item & its children
        /// </summary>
        /// <param name="parentItemPaths"></param>
        /// <param name="databaseNames"></param>
        /// <param name="languages"></param>
        /// <param name="excludeItemsWithWorkflow"></param>
        /// <param name="errorLog"></param>
        /// <returns></returns>
        private static List<ItemPublishedStatus> PublishWithSubItems(List<string> parentItemPaths, List<string> databaseNames,
            List<Language> languages, bool excludeItemsWithWorkflow, out string errorLog)
        {
            errorLog = "";
            List<ItemPublishedStatus> lstItemPublishStatus = new List<ItemPublishedStatus>();
            List<ItemPublishedStatus> tempListOfPublishedItems = new List<ItemPublishedStatus>();
            List<ItemPublishedStatus> currentItemPublishStats = new List<ItemPublishedStatus>();
            StringBuilder sbErrorLog = new StringBuilder("");
            ItemPublishedStatus itemPublishStatus = null;
            int itemsCreated = 0;
            int itemsUpdated = 0;
            int itemsSkipped = 0;
            string itemsCreatedStatus = "";
            string itemsUpdatedStatus = "";
            string itemsSkippedStatus = "";
            IEnumerable<string> uniqueItemPaths = null;

            try
            {
                if (parentItemPaths != null && parentItemPaths.Count > 0)
                {
                    Item masterDbItem = null;

                    foreach (string itemPath in parentItemPaths)
                    {
                        currentItemPublishStats = new List<ItemPublishedStatus>();
                        itemPublishStatus = new ItemPublishedStatus();
                        itemPublishStatus.ItemPath = itemPath;
                        itemsCreated = 0;
                        itemsUpdated = 0;
                        itemsSkipped = 0;
                        itemsCreatedStatus = "";
                        itemsUpdatedStatus = "";
                        itemsSkippedStatus = "";

                        ///get the item with path, from master db
                        masterDbItem = MinionHelper.Databases.masterDb.GetItem(itemPath);

                        if (masterDbItem != null)
                        {
                            ///publish this parent item
                            tempListOfPublishedItems = GetPublishedItemsStats(new List<string> { itemPath }, databaseNames, languages,
                                excludeItemsWithWorkflow, out errorLog);

                            ///collect the error logs if any
                            if (!string.IsNullOrEmpty(errorLog))
                            {
                                sbErrorLog.Append(errorLog);
                                errorLog = "";
                            }

                            if (tempListOfPublishedItems != null && tempListOfPublishedItems.Count > 0)
                            {
                                uniqueItemPaths = tempListOfPublishedItems.Select(x => x.ItemPath).Distinct();

                                if (uniqueItemPaths != null && uniqueItemPaths.Count() > 0)
                                {
                                    foreach (string path in uniqueItemPaths)
                                    {
                                        itemPublishStatus = new ItemPublishedStatus();
                                        itemPublishStatus.ItemPath = path;

                                        ///get the sum of published stats of this parent item
                                        foreach (string dbName in databaseNames)
                                        {
                                            itemsCreated = tempListOfPublishedItems.Where(x => x.ItemPath == path && x.TargetDatabase == dbName).Sum(x => x.ItemsCreated);
                                            itemsUpdated = tempListOfPublishedItems.Where(x => x.ItemPath == path && x.TargetDatabase == dbName).Sum(x => x.ItemsUpdated);
                                            itemsSkipped = tempListOfPublishedItems.Where(x => x.ItemPath == path && x.TargetDatabase == dbName).Sum(x => x.ItemsSkipped);
                                            currentItemPublishStats.Add(new ItemPublishedStatus
                                            {
                                                ItemPath = path,
                                                ItemsCreated = itemsCreated,
                                                ItemsUpdated = itemsUpdated,
                                                ItemsSkipped = itemsSkipped,
                                                TargetDatabase = dbName
                                            });
                                        }
                                    }
                                }
                            }

                            ///publish this item's children
                            if (masterDbItem.HasChildren)
                            {
                                tempListOfPublishedItems = null;
                                uniqueItemPaths = null;
                                itemsCreated = 0;
                                itemsUpdated = 0;
                                itemsSkipped = 0;
                                tempListOfPublishedItems = GetPublishedItemsStats(masterDbItem.Axes.GetDescendants().Select(x => x.Paths.FullPath).ToList(),
                                    databaseNames, languages, excludeItemsWithWorkflow, out errorLog);

                                ///collect the error logs if any
                                if (!string.IsNullOrEmpty(errorLog))
                                {
                                    sbErrorLog.Append(errorLog);
                                    errorLog = "";
                                }

                                if (tempListOfPublishedItems != null && tempListOfPublishedItems.Count > 0)
                                {
                                    ///get the sum of published stats of this parent item's children
                                    foreach (string dbName in databaseNames)
                                    {
                                        itemsCreated = tempListOfPublishedItems.Where(x => x.TargetDatabase == dbName).Sum(x => x.ItemsCreated);
                                        itemsUpdated = tempListOfPublishedItems.Where(x => x.TargetDatabase == dbName).Sum(x => x.ItemsUpdated);
                                        itemsSkipped = tempListOfPublishedItems.Where(x => x.TargetDatabase == dbName).Sum(x => x.ItemsSkipped);

                                        ///add the respective stats to that of the parent item
                                        itemsCreated += currentItemPublishStats.Where(x => x.TargetDatabase == dbName).Sum(x => x.ItemsCreated);
                                        itemsUpdated += currentItemPublishStats.Where(x => x.TargetDatabase == dbName).Sum(x => x.ItemsUpdated);
                                        itemsSkipped += currentItemPublishStats.Where(x => x.TargetDatabase == dbName).Sum(x => x.ItemsSkipped);

                                        GetItemPublishedStatsSummary(itemsCreated, itemsUpdated, itemsSkipped, dbName, out itemsCreatedStatus, out itemsUpdatedStatus, out itemsSkippedStatus);
                                        itemPublishStatus.ItemsCreatedStatus += itemsCreatedStatus;
                                        itemPublishStatus.ItemsUpdatedStatus += itemsUpdatedStatus;
                                        itemPublishStatus.ItemsSkippedStatus += itemsSkippedStatus;
                                    }
                                }
                            }
                            else
                            {
                                if (tempListOfPublishedItems != null && tempListOfPublishedItems.Count > 0)
                                {
                                    ///get the sum of published stats of this parent item
                                    foreach (string dbName in databaseNames)
                                    {
                                        itemsCreated = tempListOfPublishedItems.Where(x => x.TargetDatabase == dbName).Sum(x => x.ItemsCreated);
                                        itemsUpdated = tempListOfPublishedItems.Where(x => x.TargetDatabase == dbName).Sum(x => x.ItemsUpdated);
                                        itemsSkipped = tempListOfPublishedItems.Where(x => x.TargetDatabase == dbName).Sum(x => x.ItemsSkipped);

                                        GetItemPublishedStatsSummary(itemsCreated, itemsUpdated, itemsSkipped, dbName, out itemsCreatedStatus, out itemsUpdatedStatus, out itemsSkippedStatus);
                                        itemPublishStatus.ItemsCreatedStatus += itemsCreatedStatus;
                                        itemPublishStatus.ItemsUpdatedStatus += itemsUpdatedStatus;
                                        itemPublishStatus.ItemsSkippedStatus += itemsSkippedStatus;
                                    }
                                }
                            }
                        }
                        else
                        {
                            sbErrorLog.Append(GetErrorLog(itemPath, null, "", new Exception(ErrorMessages.SOURCE_NOT_FOUND)));

                            foreach (string dbName in databaseNames)
                            {
                                GetItemPublishedStatsSummary(0, 0, 0, dbName, out itemsCreatedStatus, out itemsUpdatedStatus, out itemsSkippedStatus);
                                itemPublishStatus.ItemsCreatedStatus += itemsCreatedStatus;
                                itemPublishStatus.ItemsUpdatedStatus += itemsUpdatedStatus;
                                itemPublishStatus.ItemsSkippedStatus += itemsSkippedStatus;
                            }
                        }

                        lstItemPublishStatus.Add(itemPublishStatus);
                    }
                }
            }
            catch (Exception ex)
            {
                sbErrorLog.Append(ex.Message);
            }

            errorLog = Convert.ToString(sbErrorLog);
            return lstItemPublishStatus;
        }

        #endregion
    }
}
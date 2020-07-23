
namespace Basiscore.Minions.sitecore.admin.minions
{
    using Basiscore.Minions.Models;
    using Basiscore.Minions.Utilities;
    using Sitecore;
    using Sitecore.Configuration;
    using Sitecore.Data;
    using Sitecore.Data.Items;
    using Sitecore.Globalization;
    using Sitecore.Install;
    using Sitecore.Install.Framework;
    using Sitecore.Install.Items;
    using Sitecore.Install.Zip;
    using Sitecore.SecurityModel;
    using Sitecore.sitecore.admin;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Web.Script.Serialization;
    using System.Web.Services;

    public partial class generatepackage : AdminPage
    {
        #region VARIABLES

        public struct ErrorMessages
        {
            public static string DATABASE_NOT_FOUND = "Database item not found";
            public static string COULD_NOT_GENERATE_PACKAGE = "Could not generate package";
        }

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

        protected void btnGeneratePackage_Click(object sender, EventArgs e)
        {
            if (MinionHelper.IsUserLoggedIn())
            {
                lblSuccess.Text = "";
                hdnFileName.Value = "";
                lblSuccess.Visible = false;
                btnDownload.Visible = false;
                lblError.Text = "";
                lblError.Visible = false;
                pnlInvalidPaths.Visible = false;
                txtInvalidPaths.Text = "";
                string databaseName = System.Convert.ToString(txtDatabase.Text).Trim();
                string packageName = System.Convert.ToString(txtPackageName.Text).Trim();
                string author = System.Convert.ToString(txtAuthor.Text).Trim();
                string version = System.Convert.ToString(txtVersion.Text).Trim();
                string itemPathsToIncludeWithSubitems = System.Convert.ToString(txtIncludeWithSubitems.Text).Trim();
                string itemPathsToInclude = System.Convert.ToString(txtIncludeItems.Text).Trim();

                try
                {
                    if (System.Convert.ToString(Session[MinionConstants.Timestamp]) == System.Convert.ToString(ViewState[MinionConstants.Timestamp]))
                    {
                        ViewState[MinionConstants.Timestamp] = DateTime.Now;
                        GeneratePackageDataModel generatePackageDataModel = new GeneratePackageDataModel();
                        generatePackageDataModel.DatabaseName = databaseName;
                        generatePackageDataModel.PackageName = packageName;
                        generatePackageDataModel.Author = author;
                        generatePackageDataModel.Version = version;
                        generatePackageDataModel.ItemPathsToIncludeWithSubitems = itemPathsToIncludeWithSubitems;
                        generatePackageDataModel.ItemPathsToInclude = itemPathsToInclude;
                        TaskStatus taskStatus = GenerateSitecorePackage(generatePackageDataModel);

                        if (taskStatus.StatusCode == 1)
                        {
                            lblSuccess.Text = taskStatus.StatusMessage;
                            hdnFileName.Value = taskStatus.FileName;
                            lblSuccess.Visible = true;
                            btnDownload.Visible = true;
                        }
                        else
                        {
                            lblError.Text = "ERROR - " + taskStatus.StatusMessage;
                            lblError.Visible = true;
                            btnDownload.Visible = false;

                            if (taskStatus.StatusCode == 3)
                            {
                                pnlInvalidPaths.Visible = true;
                                txtInvalidPaths.Text = taskStatus.InvalidPaths;
                                hdnInvalidPaths.Value = "1";
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
                catch (Exception ex)
                {
                    if (ex.Message != "Thread was being aborted.")
                    {
                        lblError.Text = "ERROR - " + ex.Message;
                        lblError.Visible = true;
                        btnDownload.Visible = false;
                    }
                }

                txtDatabase.Text = databaseName;
                txtPackageName.Text = packageName;
                txtAuthor.Text = author;
                txtVersion.Text = version;
                txtIncludeWithSubitems.Text = itemPathsToIncludeWithSubitems;
                txtIncludeItems.Text = itemPathsToInclude;
            }
            else
            {
                Response.Redirect(MinionConstants.Paths.LoginPagePath);
            }
        }

        protected void btnDownload_Click(object sender, EventArgs e)
        {
            try
            {
                if (System.Convert.ToString(Session[MinionConstants.Timestamp]) == System.Convert.ToString(ViewState[MinionConstants.Timestamp]))
                {
                    string filePath = MainUtil.MapPath(hdnFileName.Value);
                    FileInfo file = new FileInfo(filePath);

                    if (file.Exists)
                    {
                        Response.Clear();
                        Response.AddHeader("Content-Disposition", "attachment; filename=" + file.Name);
                        Response.AddHeader("Content-Length", file.Length.ToString());
                        Response.ContentType = "application/zip";
                        Response.Flush();
                        Response.TransmitFile(file.FullName);
                        Response.End();
                    }

                    Session[MinionConstants.Timestamp] = Server.UrlEncode(DateTime.Now.ToString());
                }
                else
                {
                    Response.Redirect(Request.RawUrl);
                }
            }
            catch (Exception ex)
            {
                if (ex.Message != "Thread was being aborted.")
                {

                }
            }
        }

        #endregion

        #region METHODS

        public TaskStatus GenerateSitecorePackage(GeneratePackageDataModel generatePackageDataModel)
        {
            TaskStatus taskStatus = new TaskStatus();
            string statusMessage = string.Empty;
            string fileName = string.Empty;

            if (IsValidModel(generatePackageDataModel, out statusMessage))
            {
                taskStatus = GeneratePackage(generatePackageDataModel, out statusMessage, out fileName);
            }
            else
            {
                taskStatus = new TaskStatus { StatusCode = 0, StatusMessage = statusMessage };
            }

            return taskStatus;
        }

        [WebMethod]
        public static string GenerateSitecorePackage_Ajax(GeneratePackageDataModel generatePackageDataModel)
        {
            GeneratePackageDataModel result = new GeneratePackageDataModel();
            string statusMessage = string.Empty;
            string output = string.Empty;

            try
            {
                if (MinionHelper.IsUserLoggedIn())
                {
                    if (IsValidModel(generatePackageDataModel, out statusMessage))
                    {
                        result.ResultStatus = 1;
                        result.ResultMessage = "<p class=\"success-msg\">Package generated successfully. Click <a href=\"#\">here</a> to download.</p>";
                    }
                    else
                    {
                        result.ResultStatus = 0;
                        result.ResultMessage = "<p class=\"error-msg\">ERROR - " + statusMessage + "</p>";
                    }
                }
                else
                {
                    result.ResultStatus = 2;
                }
            }
            catch (Exception ex)
            {
                result.ResultStatus = 0;
                result.ResultMessage = "<p class=\"error-msg\">ERROR - " + ex.Message + "</p>";
            }

            output = new JavaScriptSerializer().Serialize(result);
            return output;
        }

        private TaskStatus GeneratePackage(GeneratePackageDataModel generatePackageDataModel, out string statusMessage, out string fileName)
        {
            TaskStatus taskStatus = new TaskStatus();
            statusMessage = string.Empty;
            fileName = string.Empty;
            string fileNameWithPath = string.Empty;
            StringBuilder sbInvalidPaths = new StringBuilder("");

            PackageProject packageProject = new PackageProject
            {
                Metadata =
                {
                    PackageName = generatePackageDataModel.PackageName,
                    Author = generatePackageDataModel.Author,
                    Version = generatePackageDataModel.Version,
                    Publisher = ""
                }
            };

            ExplicitItemSource packageItemSource = new ExplicitItemSource
            {
                Name = "Items Source"
            };

            SourceCollection<PackageEntry> sourceCollection = new SourceCollection<PackageEntry>();
            sourceCollection.Add(packageItemSource);
            Item item = null;
            Database database = Factory.GetDatabase(generatePackageDataModel.DatabaseName);
            Item itemByLanguage = null;
            List<string> lstItemPathsToInclude = new List<string>();

            ///add items that are to be included with subitems
            foreach (string itemPath in generatePackageDataModel.GetItemPathsToIncludeWithSubitems())
            {
                item = database.Items.GetItem(itemPath);

                if (item != null)
                {
                    lstItemPathsToInclude.Add(itemPath);
                    lstItemPathsToInclude.AddRange(item.Axes.GetDescendants().Select(x => x.Paths.FullPath));
                }
                else
                {
                    sbInvalidPaths.AppendLine(itemPath);
                }
            }

            ///add items that are to be included singly
            foreach (string itemPath in generatePackageDataModel.GetItemPathsToInclude())
            {
                if (!lstItemPathsToInclude.Any(x => x.ToLower() == itemPath.ToLower()))
                {
                    lstItemPathsToInclude.Add(itemPath);
                }
            }

            if (lstItemPathsToInclude != null && lstItemPathsToInclude.Count > 0)
            {
                bool hasAtleastOneVersion = false;

                foreach (string itemPath in lstItemPathsToInclude)
                {
                    hasAtleastOneVersion = false;
                    item = database.Items.GetItem(itemPath);

                    if (item != null)
                    {
                        foreach (Language language in item.Languages)
                        {
                            itemByLanguage = item.Database.GetItem(item.ID, language);

                            if (itemByLanguage != null && itemByLanguage.Versions.Count > 0)
                            {
                                hasAtleastOneVersion = true;
                                packageItemSource.Entries.Add(new ItemReference(itemByLanguage.Uri, false).ToString());
                            }
                        }

                        ///if this item has no version but it is a template item like template/section/field, then add it to the list
                        if (!hasAtleastOneVersion && item.Paths.FullPath.StartsWith(MinionConstants.Paths.Templates))
                        {
                            packageItemSource.Entries.Add(new ItemReference(item.Uri, false).ToString());
                        }
                    }
                    else
                    {
                        sbInvalidPaths.AppendLine(itemPath);
                    }
                }
            }

            string strInvalidPaths = sbInvalidPaths.ToString();

            if (string.IsNullOrEmpty(strInvalidPaths))
            {
                if (packageItemSource.Entries.Count > 0 || sourceCollection.Sources.Count > 1)
                {
                    string packageName = generatePackageDataModel.PackageName;
                    string packageVersion = generatePackageDataModel.Version;
                    packageName = !string.IsNullOrEmpty(packageVersion) ? packageName + "-" + packageVersion : packageName;
                    packageProject.Sources.Add(sourceCollection);
                    packageProject.SaveProject = true;
                    fileName = string.Format("{0}/{1}.zip", Settings.PackagePath, packageName);
                    fileNameWithPath = MainUtil.MapPath(fileName);
                    FileInfo file = new FileInfo(fileNameWithPath);

                    if (file.Exists)
                    {
                        fileName = string.Format("{0}/{1}.zip", Settings.PackagePath, packageName + "_" + DateTime.Now.ToString("yyyyMMddhhmmss"));
                        fileNameWithPath = MainUtil.MapPath(fileName);
                    }

                    using (PackageWriter writer = new PackageWriter(fileNameWithPath))
                    {
                        using (new SecurityDisabler())
                        {
                            Sitecore.Context.SetActiveSite("shell");
                            writer.Initialize(Installer.CreateInstallationContext());
                            PackageGenerator.GeneratePackage(packageProject, writer);
                            Sitecore.Context.SetActiveSite("website");
                        }
                    }

                    statusMessage = "Package created successfully in the instance packages folder.";
                    taskStatus = new TaskStatus { StatusCode = 1, StatusMessage = statusMessage, FileName = fileName };
                }
                else
                {
                    taskStatus = new TaskStatus { StatusCode = 0, StatusMessage = "No items found to create the package.", FileName = "" };
                }
            }
            else
            {
                taskStatus = new TaskStatus { StatusCode = 3, StatusMessage = "Package not created as there are invalid item paths. Close this dialog to check the log.", FileName = "", InvalidPaths = strInvalidPaths };
            }

            return taskStatus;
        }

        private static bool IsValidModel(GeneratePackageDataModel generatePackageDataModel, out string errorMessage)
        {
            errorMessage = string.Empty;
            bool isValidModel = false;

            if (generatePackageDataModel != null)
            {
                Database database = !string.IsNullOrEmpty(generatePackageDataModel.DatabaseName) ? Factory.GetDatabase(generatePackageDataModel.DatabaseName) : null;

                if (database != null)
                {
                    if (!string.IsNullOrEmpty(generatePackageDataModel.PackageName) && MinionHelper.IsValidName(generatePackageDataModel.PackageName))
                    {
                        if (string.IsNullOrEmpty(generatePackageDataModel.Version) || MinionHelper.IsValidName(generatePackageDataModel.Version))
                        {
                            List<string> lstItemPathsToIncludeWithSubitems = generatePackageDataModel != null ? generatePackageDataModel.GetItemPathsToIncludeWithSubitems() : new List<string>();
                            List<string> lstItemPathsToPublish = generatePackageDataModel != null ? generatePackageDataModel.GetItemPathsToInclude() : new List<string>();

                            if (lstItemPathsToIncludeWithSubitems.Count > 0 || lstItemPathsToPublish.Count > 0)
                            {
                                isValidModel = true;
                            }
                            else
                            {
                                errorMessage = "No item paths specified";
                            }
                        }
                        else
                        {
                            errorMessage = "Invalid version";
                        }
                    }
                    else
                    {
                        errorMessage = "Invalid package name";
                    }
                }
                else
                {
                    errorMessage = "Invalid database";
                }
            }
            else
            {
                errorMessage = "Invalid input";
            }

            return isValidModel;
        }


        #endregion


    }
}
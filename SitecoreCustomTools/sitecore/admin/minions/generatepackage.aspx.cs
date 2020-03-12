
namespace SitecoreCustomTools.sitecore.admin.minions
{
    using Sitecore;
    using Sitecore.Configuration;
    using Sitecore.Data;
    using Sitecore.Data.Items;
    using Sitecore.Install;
    using Sitecore.Install.Framework;
    using Sitecore.Install.Items;
    using Sitecore.Install.Zip;
    using Sitecore.SecurityModel;
    using Sitecore.sitecore.admin;
    using SitecoreCustomTools.Models;
    using SitecoreCustomTools.Utilities;
    using System;
    using System.Collections.Generic;
    using System.IO;
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
            ViewState[SctConstants.Timestamp] = Session[SctConstants.Timestamp];
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (SctHelper.IsUserLoggedIn())
            {
                try
                {
                    if (!IsPostBack)
                    {
                        Session[SctConstants.Timestamp] = Server.UrlEncode(DateTime.Now.ToString());
                    }                   
                }
                catch (Exception ex)
                {

                }
            }
            else
            {
                Response.Redirect(SctConstants.Paths.LoginPagePath);
            }
        }

        protected void btnGeneratePackage_Click(object sender, EventArgs e)
        {
            if (SctHelper.IsUserLoggedIn())
            {
                try
                {
                    if (Session[SctConstants.Timestamp].ToString() == ViewState[SctConstants.Timestamp].ToString())
                    {
                        ViewState[SctConstants.Timestamp] = DateTime.Now;
                        GeneratePackageDataModel generatePackageDataModel = new GeneratePackageDataModel();
                        generatePackageDataModel.DatabaseName = txtDatabase.Text;
                        generatePackageDataModel.PackageName = txtPackageName.Text;
                        generatePackageDataModel.Author = txtAuthor.Text;
                        generatePackageDataModel.Version = txtVersion.Text;
                        generatePackageDataModel.ItemPathsToIncludeWithSubitems = txtIncludeWithSubitems.Text;
                        generatePackageDataModel.ItemPathsToInclude = txtIncludeItems.Text;
                        TaskStatus taskStatus = GenerateSitecorePackage(generatePackageDataModel);

                        if (taskStatus.StatusCode == 1)
                        {
                            lblSuccess.Text = taskStatus.StatusMessage;
                            hdnFileName.Value = taskStatus.FileName;
                            lblSuccess.Visible = true;
                        }
                        else
                        {
                            lblError.Text = "ERROR - " + taskStatus.StatusMessage;
                            lblError.Visible = true;
                            btnDownload.Visible = false;
                        }

                        hdnPostbackComplete.Value = "1";
                        Session[SctConstants.Timestamp] = Server.UrlEncode(DateTime.Now.ToString());
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
            }
            else
            {
                Response.Redirect(SctConstants.Paths.LoginPagePath);
            }
        }

        protected void btnDownload_Click(object sender, EventArgs e)
        {
            try
            {
                if (Session[SctConstants.Timestamp].ToString() == ViewState[SctConstants.Timestamp].ToString())
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

                    Session[SctConstants.Timestamp] = Server.UrlEncode(DateTime.Now.ToString());
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
                taskStatus = new TaskStatus { StatusCode = 0, StatusMessage = "ERROR - " + statusMessage };
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
                if (SctHelper.IsUserLoggedIn())
                {
                    if (IsValidModel(generatePackageDataModel, out statusMessage))
                    {
                        //GeneratePackage(generatePackageDataModel, out statusMessage);
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

            ///add items that are to be included with subitems
            foreach (string itemPath in generatePackageDataModel.GetItemPathsToIncludeWithSubitems())
            {
                item = database.Items.GetItem(itemPath);

                if (item != null)
                {
                    sourceCollection.Add(new ItemSource()
                    {
                        SkipVersions = true,
                        Database = item.Uri.DatabaseName,
                        Root = item.Uri.ItemID.ToString()
                    });
                }
            }

            ///add items that are to be included singly
            foreach (string itemPath in generatePackageDataModel.GetItemPathsToInclude())
            {
                item = database.Items.GetItem(itemPath);
                
                if (item != null)
                {
                    packageItemSource.Entries.Add(new ItemReference(item.Uri, false).ToString());
                }
            }

            if (packageItemSource.Entries.Count > 0 || sourceCollection.Sources.Count > 1)
            {
                packageProject.Sources.Add(sourceCollection);
                packageProject.SaveProject = true;
                fileName = string.Format("{0}/{1}.zip", Settings.PackagePath, generatePackageDataModel.PackageName + "-" + generatePackageDataModel.Version);
                fileNameWithPath = MainUtil.MapPath(fileName);
                FileInfo file = new FileInfo(fileNameWithPath);

                if (file.Exists)
                {
                    fileName = string.Format("{0}/{1}.zip", Settings.PackagePath, generatePackageDataModel.PackageName + "-" + generatePackageDataModel.Version + "_" + DateTime.Now.ToString("yyyyMMddhhmmss"));
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
                //statusMessage = "<p class=\"success-msg\">Package generated successfully. Click <a href=\"" + fileNameWithPath + "\">here</a> to download.</p>";
                taskStatus = new TaskStatus { StatusCode = 1, StatusMessage = "Package created successfully in the instance packages folder.", FileName = fileName };
            }
            else
            {
                taskStatus = new TaskStatus { StatusCode = 0, StatusMessage = "No items found to create the package.", FileName = "" };
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
                    if (!string.IsNullOrEmpty(generatePackageDataModel.PackageName) && SctHelper.IsValidName(generatePackageDataModel.PackageName))
                    {
                        if (string.IsNullOrEmpty(generatePackageDataModel.Version) || SctHelper.IsValidName(generatePackageDataModel.Version))
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
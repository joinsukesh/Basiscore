
namespace Basiscore.Minions.sitecore.admin.minions
{
    using Basiscore.Minions.Models;
    using Basiscore.Minions.Utilities;
    using Sitecore.Data.Engines;
    using Sitecore.Install;
    using Sitecore.Install.Files;
    using Sitecore.Install.Framework;
    using Sitecore.Install.Items;
    using Sitecore.Install.Utils;
    using Sitecore.SecurityModel;
    using System;
    using System.IO;
    using System.Linq;
    using System.Web.Script.Serialization;
    using System.Web.Services;

    public partial class installpackage : System.Web.UI.Page
    {
        #region EVENTS

        protected void Page_Load(object sender, EventArgs e)
        {
            if (MinionHelper.IsUserLoggedIn())
            {
                try
                {
                    if (!IsPostBack)
                    {
                        BindData();
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

        #endregion

        #region METHODS

        #endregion
        private void BindData()
        {
            string packagePath = Sitecore.IO.FileUtil.MapPath(Sitecore.Configuration.Settings.PackagePath);
            DirectoryInfo info = new DirectoryInfo(packagePath);
            FileInfo[] files = info.GetFiles("*.zip").OrderByDescending(p => p.CreationTime).ToArray();
            lvPackageNames.DataSource = files;
            lvPackageNames.DataBind();
        }

        [WebMethod]
        public static string InstallSitecorePackage(string packageName)
        {
            TaskStatus result = new TaskStatus();
            string error = "";
            string output = "";

            try
            {
                if (MinionHelper.IsUserLoggedIn())
                {
                    if (!string.IsNullOrEmpty(packageName))
                    {
                        InstallPackage(packageName, out error);

                        if (string.IsNullOrEmpty(error))
                        {
                            result.StatusCode = 1;
                            result.StatusMessage = "Package installed";
                        }
                        else
                        {
                            result.StatusCode = 0;
                            result.StatusMessage = error;
                        }
                    }
                    else
                    {
                        result.StatusMessage = "Invalid package name";
                        result.StatusCode = 0;
                    }
                }
                else
                {
                    result.StatusCode = 2;
                }
            }
            catch (Exception ex)
            {
                result.StatusCode = 0;
                result.StatusMessage = ex.Message;
            }

            output = new JavaScriptSerializer().Serialize(result);
            return output;
        }

        public static void InstallPackage(string packageName, out string error)
        {
            error = "";
            string sitecorePackagesPath = Sitecore.IO.FileUtil.MapPath(Sitecore.Configuration.Settings.PackagePath);
            string selectedPackagePath = sitecorePackagesPath + "\\" + packageName;
            FileInfo pkgFile = new FileInfo(selectedPackagePath);

            if (pkgFile.Exists)
            {
                using (new SecurityDisabler())
                {
                    using (new SyncOperationContext())
                    {
                        InstallMode installMode = InstallMode.Merge;
                        MergeMode mergemode = MergeMode.Append;
                        IProcessingContext context = new SimpleProcessingContext();
                        IItemInstallerEvents itemEvents = new DefaultItemInstallerEvents(new BehaviourOptions(installMode, mergemode));
                        context.AddAspect(itemEvents);
                        IFileInstallerEvents fileevents = new DefaultFileInstallerEvents(true);
                        context.AddAspect(fileevents);
                        Installer installer = new Installer();
                        installer.InstallPackage(Sitecore.MainUtil.MapPath(selectedPackagePath), context);
                    }
                }
            }
            else
            {
                error = "Package not found";
            }
        }
    }
}
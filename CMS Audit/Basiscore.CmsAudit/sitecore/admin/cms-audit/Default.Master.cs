namespace Basiscore.CmsAudit.sitecore.admin.cms_audit
{
    using Sitecore.Security.Authentication;
    using System;

    public partial class Default : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString["logout"] == Basiscore.CmsAudit.Constants.One)
            {
                AuthenticationManager.Logout();
                Response.Redirect(Constants.LoginPagePath);
            }
        }
    }
}
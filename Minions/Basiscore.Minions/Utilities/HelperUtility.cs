
namespace Basiscore.Minions.Utilities
{
    using ClosedXML.Excel;
    using HtmlAgilityPack;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Reflection;
    using System.Web;

    public class HelperUtility
    {
        /// <summary>
        /// get the host name from current URL
        /// </summary>
        /// <returns></returns>
        public static string GetHostNameFromURL()
        {
            if ((!string.IsNullOrWhiteSpace(HttpContext.Current.Request.Url.PathAndQuery)) && HttpContext.Current.Request.Url.PathAndQuery != "/")
                return HttpContext.Current.Request.Url.ToString().Replace(HttpContext.Current.Request.Url.PathAndQuery, "");
            else
                return HttpContext.Current.Request.Url.ToString();
        }

        /// <summary>
        /// get the names of all properties in a class
        /// </summary>
        /// <param name="obj">The obj<see cref="object"/></param>
        /// <returns>The <see cref="List{string}"/></returns>
        public static List<string> GetClassPropertyNames(object obj)
        {
            List<string> lstMemberNames = new List<string>();

            foreach (var prop in obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
            {
                lstMemberNames.Add(prop.Name);
            }

            return lstMemberNames;
        }

        /// <summary>
        /// gets appsetting value from web.config
        /// </summary>
        /// <param name="appsettingName"></param>
        /// <returns></returns>
        public static string GetConfigurationValue(string appsettingName)
        {
            return ConfigurationManager.AppSettings[appsettingName];
        }

        /// <summary>
        /// removes the html tags from the content
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static string StripHTML(string content)
        {
            string result = "";
            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(content);
            result = htmlDoc.DocumentNode.InnerText;
            return result;
        }

        public static void RedirectWithStatusCode(HttpContextBase httpContext, int statusCode, string relativeTargetUrl)
        {
            httpContext.Response.Clear();
            httpContext.Response.StatusCode = statusCode;
            httpContext.Response.RedirectLocation = relativeTargetUrl;
            httpContext.Response.End();
        }

        /// <summary>
        /// gets the result (string/json) from a url
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string GetResourceFromUrl(string url)
        {
            try
            {
                string resource = string.Empty;

                using (WebClient wc = new WebClient())
                {
                    resource = wc.DownloadString(url);
                }

                return resource;
            }
            catch
            {
                return string.Empty;
            }
        }

        public static string GetUserIpAddress()
        {
            HttpContext context = HttpContext.Current;
            string ipAddress = string.Empty;

            try
            {
                if (context.Request != null)
                {
                    ipAddress = context.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

                    if (string.IsNullOrWhiteSpace(ipAddress))
                    {
                        ipAddress = context.Request.UserHostAddress;
                    }

                    ipAddress = ipAddress.Split(MinionConstants.Characters.Comma).First();

                    if (string.IsNullOrWhiteSpace(ipAddress))
                    {
                        ipAddress = context.Request.ServerVariables["REMOTE_ADDR"];
                    }
                }
            }
            catch (Exception ex)
            {
                ipAddress = string.Empty;
            }

            return ipAddress;
        }

        public static bool DoesCookieExist(string cookieName)
        {
            bool cookieExists = false;

            try
            {
                if (!string.IsNullOrWhiteSpace(cookieName))
                {
                    if (HttpContext.Current.Request.Cookies.AllKeys.Contains(cookieName))
                    {
                        HttpCookie httpCookie = HttpContext.Current.Request.Cookies[cookieName];
                        cookieExists = httpCookie != null;
                    }
                }
            }
            catch (Exception)
            {
            }

            return cookieExists;
        }

        public static string GetRandomString(int stringLength)
        {
            ///This one tells you how many characters the string will contain.

            ///This one, is empty for now - but will ultimately hold the finised randomly generated password
            string newRandomString = "";

            ///This one tells you which characters are allowed in this new password
            ///Some characters like o,l, which could be visibily ambiguous to user
            string allowedChars = "";
            allowedChars = "1,2,3,4,5,6,7,8,9";
            allowedChars += "a,b,c,d,e,f,g,h,i,j,k,m,n,p,q,r,s,t,u,v,w,x,y,z";

            ///Then working with an array...

            char[] sep = { ',' };
            string[] arr = allowedChars.Split(sep);

            string temp = "";

            ///utilize the "random" class
            Random rand = new Random();

            ///and lastly - loop through the generation process...
            for (int i = 0; i < System.Convert.ToInt32(stringLength); i++)
            {
                temp = arr[rand.Next(0, arr.Length)];
                newRandomString += temp;
            }

            return newRandomString;
        }

        public static MemoryStream GetDataTableAsMemoryStream(DataTable dt, string fileNameWithExtension)
        {
            MemoryStream mStream = new MemoryStream();
            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(dt, fileNameWithExtension);
                wb.SaveAs(mStream);
                mStream.WriteTo(HttpContext.Current.Response.OutputStream);
            }

            return mStream;
        }

        public static void DownloadDataTable(DataTable dt, string httpResponseContentType, string fileNameWithExtension)
        {
            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(dt, fileNameWithExtension);
                HttpContext.Current.Response.Clear();
                HttpContext.Current.Response.Buffer = true;
                HttpContext.Current.Response.Charset = "";
                HttpContext.Current.Response.ContentType = httpResponseContentType;
                HttpContext.Current.Response.AddHeader("content-disposition", "attachment;filename=" + fileNameWithExtension);

                using (MemoryStream mStream = new MemoryStream())
                {
                    wb.SaveAs(mStream);
                    mStream.WriteTo(HttpContext.Current.Response.OutputStream);
                    HttpContext.Current.Response.Flush();
                    HttpContext.Current.Response.SuppressContent = false;
                    HttpContext.Current.ApplicationInstance.CompleteRequest();
                }
            }
        }
    }
}

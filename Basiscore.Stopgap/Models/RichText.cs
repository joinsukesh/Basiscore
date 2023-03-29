
namespace Basiscore.Stopgap.Models
{
    using Sitecore.Data.Items;
    using System.Web.Mvc;

    public class RichText : CustomItem
    {
        public RichText(Item innerItem) : base(innerItem) { }

        public string ContentId
        {
            get
            {
                return Templates.RichText.Fields.Content.ToString();
            }
        }

        public MvcHtmlString Content
        {
            get
            {
                return new MvcHtmlString(InnerItem.Fields[Templates.RichText.Fields.Content].Value);
            }
        }
    }

}
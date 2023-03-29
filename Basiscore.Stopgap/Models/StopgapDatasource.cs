
namespace Basiscore.Stopgap.Models
{
    using Sitecore;
    using Sitecore.Data.Items;
    using System.Collections.Generic;
    using System.Web.Mvc;

    public class StopgapDatasource : CustomItem
    {
        public StopgapDatasource(Item innerItem) : base(innerItem) { }

        public string HTMLId
        {
            get
            {
                return Templates.StopgapDatasource.Fields.HTML.ToString();
            }
        }

        public string HTML
        {
            get
            {
                return InnerItem.Fields[Templates.StopgapDatasource.Fields.HTML].Value;
            }
        }

        public bool IsActive
        {
            get
            {
                return InnerItem.Fields[Templates.StopgapBase.Fields.IsActive].Value == CommonConstants.One;
            }
        }

        public MvcHtmlString OutputHtml { get; set; }
    }

}
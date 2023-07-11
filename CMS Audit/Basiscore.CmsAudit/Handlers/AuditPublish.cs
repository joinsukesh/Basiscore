namespace Basiscore.CmsAudit.Handlers
{
    using Basiscore.CmsAudit.Services;
    using Sitecore.Diagnostics;
    using Sitecore.Publishing.Pipelines.Publish;
    using System;

    public class AuditPublish:PublishProcessor
    {
		public override void Process(PublishContext context)
		{
			try
			{
				new CmsAuditService().BuildPublishAuditInfo(context);
			}
			catch (Exception ex)
			{
				Log.Error(Constants.ModuleName, ex, this);
			}
		}
	}
}
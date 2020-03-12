
namespace SitecoreCustomTools.Models
{
    using Sitecore.Globalization;
    using Sitecore.Publishing;
    using Sitecore.Publishing.Pipelines.Publish;
    using Sitecore.SecurityModel;
    using System;
    using System.Collections.Generic;

    public class MyPublisher : Publisher
    {
        public MyPublisher(PublishOptions options) : base(options)
        {
        }

        public MyPublisher(PublishOptions options, IEnumerable<Language> languages) : base(options, languages)
        {
        }

        public override PublishResult PublishWithResult()
        {
            object publishLock = GetPublishLock();
            lock (publishLock)
            {
                try
                {
                    using (new SecurityDisabler())
                    {
                        AssertState();
                        PublishResult result = PerformPublishWithResult();
                        UpdateLastPublish();
                        return result;
                    }
                }
                catch (Exception ex)
                {
                    NotifyFailure(ex);
                    throw;
                }
            }
        }
    }
}
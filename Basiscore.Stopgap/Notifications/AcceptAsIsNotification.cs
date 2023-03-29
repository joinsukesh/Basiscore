namespace Basiscore.Stopgap.Notifications
{
    using Sitecore.Data.Clones;
    using Sitecore.Data.Items;
    using Sitecore.Diagnostics;

    public class AcceptAsIsNotification : Notification
    {
        private Notification InnerNotification { get; set; }

        private AcceptAsIsNotification(Notification innerNotification)
        {
            SetInnerNotification(innerNotification);
            SetProperties();
        }

        private void SetInnerNotification(Notification innerNotification)
        {
            Assert.ArgumentNotNull(innerNotification, "innerNotification");
            InnerNotification = innerNotification;
        }

        private void SetProperties()
        {
            ID = InnerNotification.ID;
            Processed = InnerNotification.Processed;
            Uri = InnerNotification.Uri;
        }

        public override void Accept(Item item)
        {
            base.Accept(item);
        }

        public override Notification Clone()
        {
            return InnerNotification.Clone();
        }

        public static Notification CreateNewAcceptAsIsNotification(Notification innerNotification)
        {
            return new AcceptAsIsNotification(innerNotification);
        }
    }
}
namespace Basiscore.Stopgap.EventHandlers
{
    using Basiscore.Stopgap.Gatherers;
    using Basiscore.Stopgap.Gatherers.Base;
    using Basiscore.Stopgap.Notifications;
    using Sitecore;
    using Sitecore.Data;
    using Sitecore.Data.Clones;
    using Sitecore.Data.Items;
    using Sitecore.Diagnostics;
    using Sitecore.Events;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class AutoCloneChildEventHandler
    {
        private static readonly IItemsGatherer ClonesGatherer = ItemClonesGatherer.CreateNewItemClonesGatherer();

        public void OnItemAdded(object sender, EventArgs args)
        {
            CloneItemIfApplicable(GetItem(args));
        }

        private static void CloneItemIfApplicable(Item item)
        {
            if (item == null)
            {
                return;
            }

            ChildCreatedNotification childCreatedNotification = CreateNewChildCreatedNotification();
            childCreatedNotification.ChildId = item.ID;
            IEnumerable<Item> clones = GetClones(item.Parent);
            foreach (Item clone in clones)
            {
                Item clonedChild = item.CloneTo(clone);
                RemoveChildCreatedNotifications(Context.ContentDatabase, clone);
            }
        }

        private static void RemoveChildCreatedNotifications(Database database, Item clone)
        {
            Assert.ArgumentNotNull(database, "database");
            Assert.ArgumentNotNull(clone, "clone");
            foreach (Notification notification in GetChildCreatedNotifications(database, clone))
            {
                AcceptNotificationAsIs(notification, clone);
            }
        }

        private static void AcceptNotificationAsIs(Notification notification, Item item)
        {
            Assert.ArgumentNotNull(notification, "notification");
            Assert.ArgumentNotNull(item, "item");
            Notification acceptAsIsNotification = AcceptAsIsNotification.CreateNewAcceptAsIsNotification(notification);
            acceptAsIsNotification.Accept(item);
        }

        private static IEnumerable<Notification> GetChildCreatedNotifications(Database database, Item clone)
        {
            Assert.ArgumentNotNull(database, "database");
            Assert.ArgumentNotNull(clone, "clone");
            return GetNotifications(database, clone).Where(notification => notification.GetType() == typeof(ChildCreatedNotification)).ToList();
        }
        private static IEnumerable<Notification> GetNotifications(Database database, Item clone)
        {
            Assert.ArgumentNotNull(database, "database");
            Assert.ArgumentNotNull(clone, "clone");
            return database.NotificationProvider.GetNotifications(clone);
        }

        private static IEnumerable<Item> GetClones(Item item)
        {
            ClonesGatherer.Source = item;
            return ClonesGatherer.Gather();
        }

        private static Item GetItem(EventArgs args)
        {
            return Event.ExtractParameter(args, 0) as Item;
        }

        private static ChildCreatedNotification CreateNewChildCreatedNotification()
        {
            return new ChildCreatedNotification();
        }
    }
}
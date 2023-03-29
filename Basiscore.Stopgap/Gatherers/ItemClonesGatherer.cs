
namespace Basiscore.Stopgap.Gatherers
{
    using Basiscore.Stopgap.Gatherers.Base;
    using Sitecore;
    using Sitecore.Data.Items;
    using Sitecore.Diagnostics;
    using Sitecore.Links;
    using System.Collections.Generic;
    using System.Linq;

    public class ItemClonesGatherer : IItemsGatherer
    {
        private LinkDatabase LinkDatabase { get; set; }

        private Item _Source;
        public Item Source
        {
            get
            {
                return _Source;
            }
            set
            {
                Assert.ArgumentNotNull(value, "Source");
                _Source = value;
            }
        }

        private ItemClonesGatherer()
        : this(GetDefaultLinkDatabase())
        {
        }

        private ItemClonesGatherer(LinkDatabase linkDatabase)
        {
            SetLinkDatabase(linkDatabase);
        }

        private ItemClonesGatherer(LinkDatabase linkDatabase, Item source)
        {
            SetLinkDatabase(linkDatabase);
            SetSource(source);
        }

        private void SetLinkDatabase(LinkDatabase linkDatabase)
        {
            Assert.ArgumentNotNull(linkDatabase, "linkDatabase");
            LinkDatabase = linkDatabase;
        }

        private void SetSource(Item source)
        {
            Source = source;
        }

        public IEnumerable<Item> Gather()
        {
            return (from itemLink in GetReferrers()
                    where IsClonedItem(itemLink)
                    select itemLink.GetSourceItem()).ToList();
        }

        private IEnumerable<ItemLink> GetReferrers()
        {
            Assert.ArgumentNotNull(Source, "Source");
            return LinkDatabase.GetReferrers(Source);
        }

        private static bool IsClonedItem(ItemLink itemLink)
        {
            return IsSourceField(itemLink)
            && !IsSourceItemNull(itemLink);
        }

        private static bool IsSourceField(ItemLink itemLink)
        {
            Assert.ArgumentNotNull(itemLink, "itemLink");
            return itemLink.SourceFieldID == FieldIDs.Source;
        }

        private static bool IsSourceItemNull(ItemLink itemLink)
        {
            Assert.ArgumentNotNull(itemLink, "itemLink");
            return itemLink.GetSourceItem() == null;
        }

        private static LinkDatabase GetDefaultLinkDatabase()
        {
            return Globals.LinkDatabase;
        }

        public static IItemsGatherer CreateNewItemClonesGatherer()
        {
            return new ItemClonesGatherer();
        }

        public static IItemsGatherer CreateNewItemClonesGatherer(LinkDatabase linkDatabase)
        {
            return new ItemClonesGatherer(linkDatabase);
        }

        public static IItemsGatherer CreateNewItemClonesGatherer(LinkDatabase linkDatabase, Item source)
        {
            return new ItemClonesGatherer(linkDatabase, source);
        }
    }
}
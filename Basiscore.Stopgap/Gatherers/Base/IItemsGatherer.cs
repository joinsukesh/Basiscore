
namespace Basiscore.Stopgap.Gatherers.Base
{
    using Sitecore.Data.Items;
    using System.Collections.Generic;

    public interface IGatherer<T, U>
    {
        T Source { get; set; }

        IEnumerable<U> Gather();
    }

    public interface IItemsGatherer : IGatherer<Item, Item>
    {
    }
}

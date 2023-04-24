
namespace Basiscore.ContentAuthoringGuide.Areas.ContentAuthoringGuide.Utilities
{
    using Sitecore.Configuration;
    using Sitecore.Data;
    using Sitecore.Data.Fields;
    using Sitecore.Data.Items;
    using Sitecore.Resources.Media;
    using Sitecore.SecurityModel;
    using System.Collections.Generic;
    using System.Linq;

    public class SitecoreUtility
    {
        #region VARIABLES

        public static Database MasterDB = Factory.GetDatabase(ConfigReferences.Master);

        #endregion

        #region METHODS

        /// <summary>
        /// check if the ID is a valid item ID
        /// </summary>
        /// <param name="id">The <see cref="ID"/></param>
        /// <returns>The <see cref="bool"/></returns>
        public static bool IsValidID(ID id)
        {
            try
            {
                return (!string.IsNullOrEmpty(System.Convert.ToString(id)) && !id.IsNull && !id.IsGlobalNullId);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// checks if an item has a field
        /// </summary>
        /// <param name="contextItem"></param>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public static bool ItemHasField(Item contextItem, ID fieldID)
        {
            //return ID.IsID(Convert.ToString(fieldID)) && contextItem != null && contextItem.Fields != null && 
            //contextItem.Fields[fieldID] != null && !string.IsNullOrEmpty(contextItem.Fields[fieldID].Name);
            contextItem.Fields.ReadAll();
            return contextItem.Fields.Any(x => x.ID == fieldID);
        }

        public static string GetMediaItemURL(Item contextItem, ID mediaItemFieldID)
        {
            //var currentItem = Sitecore.Context.Item;
            string mediaItemURL = string.Empty;

            if (contextItem != null && !mediaItemFieldID.IsNull)
            {
                ImageField imageField = contextItem.Fields[mediaItemFieldID];

                if (imageField?.MediaItem != null)
                {
                    var image = new MediaItem(imageField.MediaItem);
                    MediaUrlOptions muo = new MediaUrlOptions();
                    muo.AlwaysIncludeServerUrl = false;
                    mediaItemURL = MediaManager.GetMediaUrl(image, muo);
                    //mediaItemURL = StringUtil.EnsurePrefix('/', MediaManager.GetMediaUrl(image, muo));
                    //mediaItemURL = Sitecore.Context.GetSiteName() + mediaItemURL;
                }
            }

            return mediaItemURL;
        }

        public static string GetMediaAshxUrl(Item contextItem, ID mediaItemFieldID)
        {
            string imageUrl = string.Empty;

            if (contextItem != null && !mediaItemFieldID.IsNull)
            {
                ImageField imageField = contextItem.Fields[mediaItemFieldID];

                if (imageField?.MediaItem != null)
                {
                    MediaItem image = new MediaItem(imageField.MediaItem);

                    if (image != null)
                    {
                        string imageId = image?.InnerItem?.ID.ToString();
                        imageUrl = "/sitecore/shell/Applications/-/media/" + imageId.Replace("{", "").Replace("}", "").Replace("-", "") + ".ashx";
                    }
                }
            }

            return imageUrl;
        }
        public static IList<Item> GetMultiListValueItems(Item item, ID fieldId)
        {
            if (item != null && IsValidID(fieldId) && ItemHasField(item, fieldId))
            {
                return new MultilistField(item.Fields[fieldId]).GetItems();
            }

            return null;
        }

        /// <summary>
        /// gets an item from master database
        /// </summary>
        /// <param name="itemPathOrID"></param>
        /// <returns></returns>
        public static Item GetItem(string itemPathOrID)
        {
            if (!string.IsNullOrEmpty(itemPathOrID))
            {
                itemPathOrID = itemPathOrID.Trim().TrimEnd('/');

                if (ID.IsID(itemPathOrID))
                {
                    itemPathOrID = !itemPathOrID.StartsWith("{") ? "{" + itemPathOrID : itemPathOrID;
                    itemPathOrID = !itemPathOrID.EndsWith("}") ? itemPathOrID + "}" : itemPathOrID;
                }

                using (new SecurityDisabler())
                {
                    return MasterDB.GetItem(itemPathOrID);
                }
            }

            return null;
        }

        #endregion
    }
}
using Sitecore;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Mvc.Presentation;
using Sitecore.SecurityModel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Basiscore.Stopgap.Utilities
{
    public class SitecoreUtility
    {
        public static Item GetItem(string itemPathOrId)
        {
            return GetItemFromContextDb(itemPathOrId);
        }

        public static Item GetItemFromContextDb(string itemPathOrId)
        {
            if (!string.IsNullOrEmpty(itemPathOrId))
            {
                using (new SecurityDisabler())
                {
                    Database db = Sitecore.Context.Database;
                    return db.GetItem(itemPathOrId);
                }
            }

            return null;
        }

        /// <summary>
        /// gets the datasource of the current rendering
        /// </summary>
        /// <returns></returns>
        public static Item GetRenderingDatasourceItem()
        {
            if (RenderingContext.CurrentOrNull != null)
            {
                ///get the datasource assigned to the rendering in presentation details
                string datasourceItemPathOrId = RenderingContext.CurrentOrNull.Rendering.DataSource;

                ///get the datasource set as default in the rendering itself in its 'Data source' field.
                if (string.IsNullOrEmpty(datasourceItemPathOrId))
                {
                    datasourceItemPathOrId = RenderingContext.CurrentOrNull.Rendering.RenderingItem.DataSource;
                }

                return GetItem(datasourceItemPathOrId);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// get the active items from a multilist field and return as list of specified class
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="contextItem"></param>
        /// <param name="multilistFieldID"></param>
        /// <param name="isActiveFieldID">This is the ID of the 'Is Active' field of the featured item</param>
        /// <returns></returns>
        public static List<T> GetActiveItemsFromMultilistField<T>(Item contextItem, ID multilistFieldID, ID isActiveFieldID) where T : class
        {
            List<T> activeItems = new List<T>();
            List<Item> selectedItems = null;

            ///get the selected items selected in the multilist field
            selectedItems = GetSelectedItemsInMultilistField(contextItem, multilistFieldID)
                    .Where(x => isActiveFieldID.IsNull || isActiveFieldID.IsGlobalNullId || x.Fields[isActiveFieldID].Value == CommonConstants.One).ToList();

            if (selectedItems != null && selectedItems.Count > 0)
            {
                foreach (Item item in selectedItems)
                {
                    T instance = (T)Activator.CreateInstance(typeof(T), args: item);

                    if (instance != null)
                    {
                        activeItems.Add(instance);
                    }
                }
            }

            return activeItems;
        }

        /// <summary>
        /// gets the list of selected items in a multilist field
        /// </summary>
        /// <param name="contextItem">The contextItem<see cref="Item"/></param>
        /// <param name="multilistFieldID">The multilistFieldID<see cref="ID"/></param>
        /// <returns></returns>
        public static IList<Item> GetSelectedItemsInMultilistField(Item contextItem, ID multilistFieldID)
        {
            if (contextItem != null)
            {
                return new MultilistField(contextItem.Fields[multilistFieldID]).GetItems();
            }

            return null;
        }

        /// <summary>
        /// returns true if the page is being viewed in experience editor mode
        /// </summary>
        /// <returns></returns>
        public static bool IsExperienceEditorMode()
        {
            return Context.PageMode.IsExperienceEditor || Context.PageMode.IsExperienceEditorEditing;
        }

        /// <summary>
        /// returns true if the page is being viewed in experience editor mode
        /// </summary>
        /// <returns></returns>
        public static bool IsPreviewMode()
        {
            return Context.PageMode.IsPreview;
        }
    }
}
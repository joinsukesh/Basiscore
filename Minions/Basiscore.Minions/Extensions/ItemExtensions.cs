
namespace Basiscore.Minions.Extensions
{
    using Basiscore.Minions.Utilities;
    using Sitecore;
    using Sitecore.Configuration;
    using Sitecore.Data;
    using Sitecore.Data.Fields;
    using Sitecore.Data.Items;
    using Sitecore.Data.Managers;
    using Sitecore.Data.Templates;
    using Sitecore.Globalization;
    using Sitecore.Layouts;
    using Sitecore.Links;
    using Sitecore.Links.UrlBuilders;
    using Sitecore.Publishing;
    using Sitecore.Resources.Media;
    using Sitecore.Security.Accounts;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Security;

    public static class ItemExtensions
    {

        #region ITEM

        /// <summary>
        /// checks if an item has a field
        /// </summary>
        /// <param name="contextItem"></param>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public static bool ItemHasField(this Item item, ID fieldId)
        {
            return item.Template.Fields.Any(x => x.ID == fieldId);
        }

        /// <summary>
        /// Returns user-friendly URL of an item
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static string Url(this Item item)
        {
            return LinkManager.GetItemUrl(item);
        }

        public static string Url(this Item item, ItemUrlBuilderOptions options)
        {
            return LinkManager.GetItemUrl(item, options);
        }

        /// <summary>
        /// gets the url of the media item (e.g image)
        /// </summary>
        /// <param name="contextItem"></param>
        /// <param name="mediaItemFieldID"></param>
        /// <param name="includeServerURL"></param>
        /// <returns></returns>
        public static string Url(this Item item, bool isMediaItem, bool includeServerURL = false)
        {
            string url = string.Empty;

            if (item != null)
            {
                if (isMediaItem)
                {
                    MediaItem image = new MediaItem(item);
                    MediaUrlBuilderOptions muo = new MediaUrlBuilderOptions
                    {
                        AlwaysIncludeServerUrl = includeServerURL
                    };
                    url = MediaManager.GetMediaUrl(image, muo);
                }
                else
                {
                    url = LinkManager.GetItemUrl(item);
                }
            }

            return url;
        }

        public static string RelativePath(this Item item)
        {
            string itemPath = item.Paths.Path.ToLower();
            itemPath = itemPath.Replace(Sitecore.Context.Data.Site.RootPath.ToLower(), "")
            .Replace(Sitecore.Context.Data.Site.StartItem.ToLower(), "");
            return itemPath;
        }

        public static bool IsContentItem(this Item item)
        {
            return item.Paths.IsContentItem;
        }

        public static bool IsMediaItem(this Item item)
        {
            return item.Paths.IsMediaItem;
        }

        /// <summary>
        /// Gets an array of all hierarchial parent items of a current item. 
        /// Can be useful for creating breadcrumbs.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="descending"></param>
        /// <returns></returns>
        public static IEnumerable<Item> ParentList(this Item item, Boolean descending = true)
        {
            if (item == null)
                return null;

            List<Item> parents = new List<Item>();

            Item currentItem = item.Parent;
            while (currentItem != null)
            {
                parents.Add(currentItem);


                currentItem = currentItem.Parent;
            }

            if (descending)
                parents.Reverse();

            return parents;
        }

        /// <summary>
        /// Get referrers of an item using the link database
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static IEnumerable<Item> GetReferrersAsItems(this Item item)
        {
            var links = Globals.LinkDatabase.GetReferrers(item);
            return links.Select(i => i.GetTargetItem()).Where(i => i != null);
        }

        /// <summary>
        /// Tells, if a given item derives from a specific template
        /// </summary>
        /// <param name="item"></param>
        /// <param name="templateId"></param>
        /// <returns></returns>
        public static bool IsDerived(this Item item, ID templateId)
        {
            if (item == null)
                return false;

            if (templateId.IsNull)
                return false;

            TemplateItem templateItem = item.Database.Templates[templateId];

            bool returnValue = false;
            if (templateItem != null)
            {
                Template template = TemplateManager.GetTemplate(item);

                returnValue = template != null && template.ID == templateItem.ID || template.DescendsFrom(templateItem.ID);
            }

            return returnValue;
        }

        /// <summary>
        /// get all the child items of a given template id, under a parent item
        /// </summary>
        /// <param name="parentItem"></param>
        /// <param name="templateID">The templateID<see cref="ID"/></param>
        /// <param name="checkBaseTemplates">The checkBaseTemplates<see cref="bool"/></param>
        /// <returns></returns>
        public static List<Item> GetChildItemsByTemplate(this Item item, ID templateId, bool checkBaseTemplates = false, string database = "")
        {
            List<Item> itemsByTemplate = new List<Item>();

            if (item != null)
            {
                List<ID> usages = new List<ID>();
                TemplateItem selectedTemplateItem = MinionHelper.GetItem(templateId.ToString(), database);

                if (checkBaseTemplates)
                {
                    itemsByTemplate = item.Axes.GetDescendants().Where(x => x.TemplateID == templateId ||
                    (x.Template != null && x.Template.BaseTemplates.Any(b => b.ID == templateId))).ToList();
                }
                else
                {
                    itemsByTemplate = item.Axes.GetDescendants().Where(x => x.TemplateID == templateId).ToList();
                }
            }

            return itemsByTemplate;
        }

        public static List<T> GetChildItemsByTemplate<T>(this Item item, ID templateId, bool checkBaseTemplates = false, string database = "") where T : class
        {
            List<T> activeItems = null;
            List<Item> itemsByTemplate = item.GetChildItemsByTemplate(templateId, checkBaseTemplates, database);

            if (itemsByTemplate != null)
            {
                activeItems = MinionHelper.GetClassObjectsFromItems<T>(itemsByTemplate);
            }

            return activeItems;
        }

        /// <summary>
        /// Identifies if current item has a child with a specific ID
        /// </summary>
        /// <param name="item"></param>
        /// <param name="childId"></param>
        /// <returns></returns>
        public static bool HasChild(this Item item, ID childId)
        {
            return item.Children.Any(x => x.ID == childId);
        }

        /// <summary>
        /// Identifies if current item has a decscendant with a specific ID
        /// </summary>
        /// <param name="item"></param>
        /// <param name="descendantId"></param>
        /// <returns></returns>
        public static bool HasDescendant(this Item item, ID descendantId)
        {
            foreach (Item child in item.Children)
            {
                if (child.ID == descendantId)
                    return true;

                HasDescendant(child, descendantId);
            }
            return false;
        }

        #endregion

        #region LANGUAGE

        /// <summary>
        /// This extension method will be used to check ifitem exist in current provided language 
        /// </summary>
        /// <param name="item"></param>
        /// <param name="languageToCheck"></param>
        /// <returns></returns>
        public static bool DoesLanguageVersionExist(this Item item, Language languageToCheck)
        {
            bool isExist = false;
            Sitecore.Data.Items.Item itemInLanguage = item.Database.GetItem(item.ID, languageToCheck);
            isExist = itemInLanguage.Versions.Count > 0;
            return isExist;
        }

        /// <summary>
        /// Tells, if an Item has a version in a specific language
        /// </summary>
        /// <param name="item"></param>
        /// <param name="languageName"></param>
        /// <returns></returns>
        public static bool HasLanguage(this Item item, string languageName)
        {
            return ItemManager.GetVersions(item, LanguageManager.GetLanguage(languageName)).Count > 0;
        }

        /// <summary>
        /// Tells, if an Item has a version in a specific language
        /// </summary>
        /// <param name="item"></param>
        /// <param name="language"></param>
        /// <returns></returns>
        public static bool HasLanguage(this Item item, Language language)
        {
            return ItemManager.GetVersions(item, language).Count > 0;
        }

        /// <summary>
        /// Gives the amount of versions that a given item has in a given language
        /// </summary>
        /// <param name="item"></param>
        /// <param name="lang"></param>
        /// <returns></returns>
        public static int LanguageVersionCount(this Item item, Language lang)
        {
            if (item == null)
                return 0;
            Item currentItem = item.Database.GetItem(item.ID, lang);
            if (currentItem.Versions.Count > 0)
                return currentItem.Versions.Count;
            else
                return 0;
        }

        #endregion

        #region FIELD

        public static Item GetInternalLinkFieldItem(this Item item, string fieldId)
        {
            if (item != null)
            {
                InternalLinkField ilf = item.Fields[fieldId];
                if (ilf != null && ilf.TargetItem != null)
                {
                    return ilf.TargetItem;
                }
            }
            return null;
        }

        /// <summary>
        /// Gets value of Checkbox field id
        /// </summary>
        /// <param name="item"></param>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public static bool IsChecked(this Item item, ID fieldId)
        {
            Field fieldValue = item.Fields[fieldId];

            if (fieldValue != null)
                return (fieldValue.Value == "1" ? true : false);
            else
                return false;
        }

        /// <summary>
        /// gets the url of the media item (e.g image)
        /// </summary>
        /// <param name="contextItem"></param>
        /// <param name="mediaItemFieldID"></param>
        /// <param name="includeServerURL"></param>
        /// <returns></returns>
        public static string GetMediaItemUrl(this Item item, ID mediaItemFieldId, bool includeServerURL = false)
        {
            string mediaItemURL = string.Empty;

            if (item != null)
            {
                ImageField imageField = item.Fields[mediaItemFieldId];

                if (imageField?.MediaItem != null)
                {
                    MediaItem image = new MediaItem(imageField.MediaItem);
                    MediaUrlBuilderOptions muo = new MediaUrlBuilderOptions
                    {
                        AlwaysIncludeServerUrl = includeServerURL
                    };
                    mediaItemURL = MediaManager.GetMediaUrl(image, muo);
                }
            }

            return mediaItemURL;
        }

        /// <summary>
        /// get the text of the alt field in the sitecore image
        /// </summary>
        /// <param name="contextItem"></param>
        /// <param name="mediaItemFieldID"></param>
        /// <returns></returns>
        public static string GetImageAltText(this Item item, ID mediaItemFieldId)
        {
            ImageField image = (ImageField)item.Fields[mediaItemFieldId];
            return image != null ? image.Alt : string.Empty;
        }

        /// <summary>
        /// get the text of the Title field in the sitecore image
        /// </summary>
        /// <param name="contextItem"></param>
        /// <param name="mediaItemFieldID"></param>
        /// <returns></returns>
        public static string GetImageTitleText(this Item item, ID mediaItemFieldId)
        {
            ImageField image = (ImageField)item.Fields[mediaItemFieldId];
            return image != null ? image.MediaItem.Fields["Title"].ToString() : string.Empty;
        }

        /// <summary>
        /// Returns MediaItem from Media Field id
        /// </summary>
        /// <param name="item"></param>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public static MediaItem GetMediaItem(this Item item, ID fieldId)
        {
            Field fieldValue = item.Fields[fieldId];
            MediaItem mediaItem = null;

            if ((fieldValue != null) && (fieldValue.HasValue))
                mediaItem = ((ImageField)fieldValue).MediaItem;

            return mediaItem;
        }

        /// <summary>
        /// Retrieves value of a string-based field - Single Line, Multiline, etc, by field id.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public static String GetString(this Item item, ID fieldId)
        {
            string fieldValue = string.Empty;
            if (item != null)
            {
                Field field = item.Fields[fieldId];
                if ((field != null) && (!string.IsNullOrWhiteSpace(field.Value)))
                    fieldValue = field.Value;
            }
            return fieldValue;
        }

        /// <summary>
        /// Gets DateTime value out of Date of DateTime field id
        /// </summary>
        /// <param name="item"></param>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public static DateTime GetDate(this Item item, ID fieldId)
        {
            DateTime dateTime = DateTime.Now;
            DateField fieldValue = item.Fields[fieldId];

            if ((fieldValue != null) && (fieldValue.InnerField.Value != null))
                dateTime = fieldValue.DateTime;

            return dateTime;
        }

        /// <summary>
        /// Gets the value of HtmlField by field id
        /// </summary>
        /// <param name="item"></param>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public static HtmlField GetHtml(this Item item, ID fieldId)
        {
            HtmlField htmlField = item.Fields[fieldId];
            return htmlField;
        }

        /// <summary>
        /// Gets LInk Field out of corresponding item
        /// </summary>
        /// <param name="item"></param>
        /// <param name="fieldId"></param>
        /// <param name="getURLWithDomain"></param>
        /// <returns></returns>
        public static string GetLinkFieldUrl(this Item item, ID fieldId, bool getURLWithDomain = false, string database = "")
        {
            string url = string.Empty;
            if (item == null || fieldId.IsNull || string.IsNullOrWhiteSpace(System.Convert.ToString(fieldId)))
            {
                return string.Empty;
            }

            LinkField linkField = item.Fields[fieldId.ToString()];

            if (linkField == null)
            {
                return string.Empty;
            }

            switch (linkField.LinkType.ToLower())
            {
                case "internal":
                    /// Use LinkMananger for internal links, if link is not empty
                    Item targetItem = linkField.TargetItem;

                    if (targetItem == null)
                    {
                        targetItem = MinionHelper.GetItem(System.Convert.ToString(linkField.TargetID), database);
                    }

                    url = targetItem != null ? LinkManager.GetItemUrl(targetItem) : string.Empty;
                    break;
                case "media":
                    /// Use MediaManager for media links, if link is not empty
                    Item mediaItem = linkField.TargetItem;

                    if (mediaItem == null)
                    {
                        mediaItem = MinionHelper.GetItem(System.Convert.ToString(linkField.TargetID), database);
                    }

                    url = mediaItem != null ? MediaManager.GetMediaUrl(mediaItem) : string.Empty;
                    break;
                case "external":
                    /// Just return external links
                    url = linkField.Url;
                    break;
                case "anchor":
                    /// Prefix anchor link with # if link if not empty
                    url = !string.IsNullOrWhiteSpace(linkField.Anchor) ? "#" + linkField.Anchor : string.Empty;
                    break;
                case "mailto":
                    /// Just return mailto link
                    url = linkField.Url;
                    break;
                case "javascript":
                    /// Just return javascript
                    url = linkField.Url;
                    break;
                default:
                    /// Just please the compiler, this
                    /// condition will never be met
                    url = linkField.Url;
                    break;
            }

            if ((!string.IsNullOrWhiteSpace(url)) && getURLWithDomain)
            {
                url = url.StartsWith("/") ? url : "/" + url;
                url = HelperUtility.GetHostNameFromURL() + url;
            }

            return url;
        }

        /// <summary>
        /// gets the text entered in the "Description" field of a general link field
        /// </summary>
        /// <param name="item"></param>
        /// <param name="fieldId"></param>
        /// <returns></returns>
        public static string GetLinkDescription(this Item item, ID fieldId)
        {
            if (item == null || fieldId.IsNull || string.IsNullOrWhiteSpace(System.Convert.ToString(fieldId)))
            {
                return string.Empty;
            }

            LinkField field = item.Fields[fieldId];
            return field != null ? field.Text : string.Empty;
        }

        /// <summary>
        /// Checks whether a given link is a specific type
        /// </summary>
        /// <param name="item"></param>
        /// <param name="fieldId"></param>
        /// <param name="linkType"></param>
        /// <returns></returns>
        public static bool IsSameGeneralLinkType(this Item item, ID fieldId, string linkType)
        {
            bool matchesLinkType = false;
            LinkField linkField = null;

            if (item != null && !string.IsNullOrWhiteSpace(linkType))
            {
                linkField = item.Fields[fieldId.ToString()];

                if (linkField != null && linkField.LinkType.ToLower() == linkType.ToLower())
                {
                    matchesLinkType = true;
                }
            }

            return matchesLinkType;
        }

        /// <summary>
        /// get the text entered in the "Class" field of a general link field
        /// </summary>
        /// <param name="item"></param>
        /// <param name="fieldId"></param>
        /// <returns></returns>
        public static string GetLinkClass(this Item item, ID fieldId)
        {
            if (item == null || fieldId.IsNull || string.IsNullOrWhiteSpace(System.Convert.ToString(fieldId)))
            {
                return string.Empty;
            }

            LinkField field = item.Fields[fieldId];
            return field != null ? field.Class : string.Empty;
        }

        /// <summary>
        /// Gets the text in the anchor field of the general link item
        /// </summary>
        /// <param name="item"></param>
        /// <param name="fieldId"></param>
        /// <returns></returns>
        public static string GetLinkAnchor(this Item item, ID fieldId)
        {
            if (item == null || fieldId.IsNull || string.IsNullOrWhiteSpace(System.Convert.ToString(fieldId)))
            {
                return string.Empty;
            }

            LinkField field = item.Fields[fieldId];
            return (field != null && !string.IsNullOrWhiteSpace(field.Anchor)) ? "#" + field.Anchor : string.Empty;
        }

        /// <summary>
        /// get the target type of link i.e the value of the "target" attribute in teh raw value
        /// </summary>
        /// <param name="item"></param>
        /// <param name="fieldId"></param>
        /// <returns></returns>
        public static string GetLinkTargetType(this Item item, ID fieldId)
        {
            if (item == null || fieldId.IsNull || string.IsNullOrWhiteSpace(System.Convert.ToString(fieldId)))
            {
                return string.Empty;
            }

            LinkField field = item.Fields[fieldId];
            return field != null ? field.Target : string.Empty;
        }

        /// <summary>
        /// Checks if external link is selected in the General Link field
        /// </summary>
        /// <param name="item"></param>
        /// <param name="fieldId"></param>
        /// <returns></returns>
        public static bool IsExternalLink(Item item, ID generalLinkFieldId)
        {
            return item.IsSameGeneralLinkType(generalLinkFieldId, MinionConstants.LinkTypes.External) || item.GetLinkTargetType(generalLinkFieldId) == MinionConstants.LinkTargetTypes.Blank;
        }

        /// <summary>
        /// Gets Reference Field out of corresponding item - Droplink, Droptree, Grouped Droplink etc.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="fieldId"></param>
        /// <returns></returns>
        public static ReferenceField GetReferenceField(this Item item, ID fieldId)
        {
            ReferenceField referenceField = item.Fields[fieldId];
            return referenceField;
        }

        /// <summary>
        /// Gets an associated item out of Reference Field of an item - Droplink, Droptree, Grouped Droplink etc.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="fieldId"></param>
        /// <returns></returns>
        public static Item GetReferenceFieldItem(this Item item, ID fieldId)
        {
            Item targetItem = null;
            ReferenceField referenceField = item.Fields[fieldId];
            if ((referenceField != null) && (referenceField.TargetItem != null))
                targetItem = referenceField.TargetItem;

            return targetItem;
        }

        /// <summary>
        /// Gets the raw value of the selected item in a Droplink, Droptree, Grouped Droplink etc.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="fieldId"></param>
        /// <returns></returns>
        public static string GetReferenceFieldValue(this Item item, ID fieldId)
        {
            return item?.Fields[fieldId].Value;
        }

        /// <summary>
        /// gets the text value of the selected item in a reference field like Droplink, Droptree
        /// </summary>
        /// <param name="item"></param>
        /// <param name="fieldId"></param>
        /// <returns></returns>
        public static string GetReferenceFieldItemValue(this Item item, ID fieldId, ID templateFieldId)
        {
            Item targetItem = item.GetReferenceFieldItem(fieldId);
            if (targetItem != null)
                return targetItem.Fields[templateFieldId].Value;

            return string.Empty;
        }

        /// <summary>
        /// Returns Multilist Field
        /// </summary>
        /// <param name="item"></param>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public static MultilistField GetMultilistField(this Item item, ID fieldId)
        {
            MultilistField multilistField = item.Fields[fieldId];
            return multilistField;
        }

        /// <summary>
        /// Returns associated items of Multilist Field; works also with Checklist, Treelist, Treelist-Ex
        /// </summary>
        /// <param name="item"></param>
        /// <param name="fieldId"></param>
        /// <returns></returns>
        public static Item[] GetMultilistFieldItems(this Item item, ID fieldId)
        {
            Item[] targetItems = null;
            MultilistField multilistField = item.Fields[fieldId];
            if ((multilistField != null) && (multilistField.InnerField != null))
                targetItems = multilistField.GetItems();

            return targetItems;
        }

        #endregion

        #region GET DATASOURCE ITEMS OF A SITECORE ITEM

        public static List<Item> GetDataSourceItems(this Item item)
        {
            List<Item> list = new List<Item>();
            foreach (RenderingReference reference in item.GetRenderingReferences())
            {
                Item dataSourceItem = reference.GetDataSourceItem();
                if (dataSourceItem != null)
                {
                    list.Add(dataSourceItem);
                }
            }
            return list;
        }

        public static Item GetDataSourceItem(this RenderingReference reference)
        {
            if (reference != null)
            {
                return GetDataSourceItem(reference.Settings.DataSource, reference.Database);
            }
            return null;
        }

        private static Item GetDataSourceItem(string id, Database db)
        {
            Guid itemId;
            return Guid.TryParse(id, out itemId)
            ? db.GetItem(new ID(itemId))
            : db.GetItem(id);
        }

        /// <summary>
        ///  For items that have been added through personalization and rules
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public static List<Item> GetPersonalizationDataSourceItems(this Item item)
        {
            List<Item> list = new List<Item>();
            foreach (RenderingReference reference in item.GetRenderingReferences())
            {
                list.AddRange(reference.GetPersonalizationDataSourceItem());
            }
            return list;
        }

        /// <summary>
        /// For item that has been added through personalization and rules
        /// </summary>
        /// <param name="reference"></param>
        /// <returns></returns>
        private static List<Item> GetPersonalizationDataSourceItem(this RenderingReference reference)
        {
            List<Item> list = new List<Item>();
            if (reference != null && reference.Settings.Rules != null && reference.Settings.Rules.Count > 0)
            {
                foreach (var r in reference.Settings.Rules.Rules)
                {
                    foreach (var a in r.Actions)
                    {
                        var setDataSourceAction = a as Sitecore.Rules.ConditionalRenderings.SetDataSourceAction<Sitecore.Rules.ConditionalRenderings.ConditionalRenderingsRuleContext>;
                        if (setDataSourceAction != null)
                        {
                            Item dataSourceItem = GetDataSourceItem(setDataSourceAction.DataSource, reference.Database);
                            if (dataSourceItem != null)
                            {
                                list.Add(dataSourceItem);
                            }
                        }
                    }
                }
            }
            return list;
        }

        #endregion

        #region TEMPLATE

        /// <summary>
        /// Checks wether specific item is a template item
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static bool IsTemplate(this Item item)
        {
            return item.Database.Engines.TemplateEngine.IsTemplatePart(item);
        }

        /// <summary>
        /// Returns true if current template item is derived from another template 
        /// </summary>
        /// <param name="me"></param>
        /// <param name="templateID"></param>
        /// <param name="includeSelf"></param>
        /// <param name="recursive"></param>
        /// <returns></returns>
        public static bool HasBaseTemplate(this TemplateItem me, ID templateID, bool includeSelf = false, bool recursive = true)
        {
            if (includeSelf && me.ID == templateID)
            {
                return true;
            }

            if (recursive)
            {
                foreach (TemplateItem baseTemplate in me.BaseTemplates)
                {
                    if (baseTemplate.HasBaseTemplate(
                    templateID,
                    true /*includeSelf*/,
                    true /*recursive*/))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Indicates whether an item is a standard value item or not
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static bool IsStandardValues(this Item item)
        {
            if (item == null)
                return false;
            bool isStandardValue = false;

            if (item.Template.StandardValues != null)
                isStandardValue = (item.Template.StandardValues.ID == item.ID);

            return isStandardValue;
        }

        #endregion

        #region RENDERING

        /// <summary>
        /// Indicates if an item has versioned renderings.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static bool HasRenderings(this Item item)
        {
            return item.Visualization.GetRenderings(Context.Device, false).Any();
        }

        public static RenderingReference[] GetRenderingReferences(this Item item)
        {
            if (item == null)
            {
                return new RenderingReference[0];
            }
            return item.Visualization.GetRenderings(Sitecore.Context.Device, false);
        }

        /// <summary>
        /// Indicates if an item has versioned renderings.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static bool HasVersionedRenderings(this Item item)
        {
            if (item.Fields[FieldIDs.FinalLayoutField] == null)
                return false;

            var field = item.Fields[FieldIDs.FinalLayoutField];
            return !string.IsNullOrWhiteSpace(field.GetValue(false, false));
        }

        /// <summary>
        /// Indicates if an item has versioned renderings for a language
        /// </summary>
        /// <param name="item"></param>
        /// <param name="language"></param>
        /// <returns></returns>
        public static bool HasVersionedRenderingsOnLanguage(this Item item, Language language)
        {
            return item != null && item.Database.GetItem(item.ID, language).HasVersionedRenderings();
        }

        /// <summary>
        /// Indicates if an item has versioned renderings on any language
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static bool HasVersionedRenderingsOnAnyLanguage(this Item item)
        {
            return ItemManager.GetContentLanguages(item).Any(item.HasVersionedRenderingsOnLanguage);
        }

        /// <summary>
        /// Indicates if an item has versioned renderings on context language
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static bool HasVersionedRenderingsOnContextLanguage(this Item item)
        {
            return item.HasVersionedRenderingsOnLanguage(Context.Language);
        }

        #endregion

        #region PUBLISH

        /// <summary>
        /// Performs publishing of an item. Deep parameter makes publishing recursive against all the subitems
        /// </summary>
        /// <param name="item"></param>
        /// <param name="deep"></param>
        //public static void Publish(this Item item, bool deep, string targetDatabaseName = MinionConstants.DatabaseNames.Web)
        //{
        //    var publishOptions = new PublishOptions(item.Database,
        //    Database.GetDatabase(targetDatabaseName),
        //    PublishMode.SingleItem,
        //    item.Language,
        //    DateTime.Now);
        //    var publisher = new Publisher(publishOptions);
        //    publisher.Options.RootItem = item;
        //    publisher.Options.Deep = deep;
        //    publisher.Publish();
        //}

        /// <summary>
        /// Unpublishes current item
        /// </summary>
        /// <param name="item"></param>
        //public static void UnPublish(this Item item)
        //{
        //    item.Publishing.UnpublishDate = DateTime.Now;
        //    item.Publish(true);
        //}

        /// <summary>
        /// Finds out if item is already published
        /// </summary>
        /// <param name="pItem"></param>
        /// <returns></returns>
        //public static bool IsPublished(this Item pItem, string targetDatabaseName = MinionConstants.DatabaseNames.Web)
        //{
        //    Database lWebDb = Factory.GetDatabase(targetDatabaseName);
        //    if (pItem != null && lWebDb != null)
        //    {
        //        Item lWebItem = lWebDb.GetItem(pItem.ID, pItem.Language, pItem.Version);
        //        if (lWebItem == null || pItem.Statistics.Updated > lWebItem.Statistics.Updated)
        //        {
        //            return false;
        //        }
        //    }
        //    return true;
        //}

        #endregion

        #region USER

        public static MembershipUser GetMembershipUser(this Account account)
        {
            return Membership.GetUser(account.Name);
        }

        #endregion

        #region DICTIONARY

        /// <summary>
        /// Returns the value of the key field
        /// Works for dictionary item only, i.e item created with DictionaryEntry template
        /// </summary>        
        public static string GetDictionaryKey(this Item item)
        {
            string key = string.Empty;

            if (item != null && item.TemplateID == MinionConstants.Templates.DictionaryEntry.ID)
            {
                key = item.Fields[MinionConstants.Templates.DictionaryEntry.Fields.Key].Value;
            }

            return key;
        }

        /// <summary>
        /// Returns the value of the phrase field
        /// Works for dictionary item only, i.e item created with DictionaryEntry template
        /// </summary>        
        public static string GetDictionaryPhrase(this Item item)
        {
            string phrase = string.Empty;

            if (item != null && item.TemplateID == MinionConstants.Templates.DictionaryEntry.ID)
            {
                phrase = item.Fields[MinionConstants.Templates.DictionaryEntry.Fields.Phrase].Value;
            }

            return phrase;
        }

        #endregion


    }
}

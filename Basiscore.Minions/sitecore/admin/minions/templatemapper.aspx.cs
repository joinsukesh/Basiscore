
namespace Basiscore.Minions.sitecore.admin.minions
{
    using Sitecore.Data.Items;
    using Basiscore.Minions.Models;
    using Basiscore.Minions.Utilities;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Web.Script.Serialization;
    using System.Web.Services;

    public partial class templatemapper : System.Web.UI.Page
    {
        #region EVENTS      

        protected void Page_Load(object sender, EventArgs e)
        {
            if (MinionHelper.IsUserLoggedIn())
            {
                try
                {

                }
                catch (Exception ex)
                {

                }
            }
            else
            {
                Response.Redirect(MinionConstants.Paths.LoginPagePath);
            }
        }

        [WebMethod]
        public static string GetTemplateMapperResults(TemplateMapperDataModel templateMapperDataModel)
        {
            string output = "";
            TemplateMapperResult result = new TemplateMapperResult();

            if (MinionHelper.IsUserLoggedIn())
            {
                try
                {
                    Item templateItem = GetTemplate(templateMapperDataModel.TemplateId);

                    if (templateItem != null)
                    {
                        result.Status = 1;
                        result.TemplateReferences = BuildTemplateReferencesString(templateItem);
                        result.TemplateClass = BuildMapperClassString(templateItem, templateMapperDataModel.TemplateStructPrefix);
                    }
                    else
                    {
                        result.Status = 0;
                        result.Error = "Invalid template Id";
                    }
                }
                catch (Exception ex)
                {
                    result.Status = 0;
                    result.Error = ex.Message;
                }

                output = new JavaScriptSerializer().Serialize(result);
            }
            else
            {
                result.Status = 2;
            }

            return output;
        }

        #endregion

        #region METHODS

        private static Item GetTemplate(string templateId)
        {
            Item templateItem = null;
            templateId = templateId.Trim();

            if (!string.IsNullOrEmpty(templateId))
            {
                templateItem = MinionHelper.Databases.masterDb.GetItem(templateId);

                if (templateItem == null || templateItem.TemplateID != MinionConstants.Templates.Template.ID)
                {
                    templateItem = null;
                }
            }

            return templateItem;
        }

        /// <summary>
        /// build the id mapper string structure
        /// </summary>
        /// <param name="lstTemplateItems"></param>
        /// <returns></returns>
        private static string BuildTemplateReferencesString(Item templateItem)
        {
            StringBuilder sb = new StringBuilder(string.Empty);
            IEnumerable<Item> fields = MinionHelper.GetItemsByTemplate(templateItem, MinionConstants.Templates.TemplateField.ID);

            sb.AppendLine("public struct " + templateItem.Name.Trim().Replace(" ", ""));
            sb.AppendLine("{");
            sb.AppendLine("  public static readonly ID ID = new ID(\"" + templateItem.ID + "\");");

            if (fields != null && fields.Count() > 0)
            {
                sb.AppendLine();
                sb.AppendLine("  public struct Fields");
                sb.AppendLine("  {");

                foreach (Item field in fields)
                {
                    sb.AppendLine("    public static readonly ID " + field.Name.Trim().Replace(" ", "") + " = new ID(\"" + field.ID + "\");");
                }

                sb.AppendLine("  }");
            }

            sb.AppendLine("}");
            sb.AppendLine();

            return sb.ToString();
        }

        /// <summary>
        /// build the class string structure
        /// </summary>
        /// <param name="lstTemplateItems"></param>
        /// <returns></returns>
        private static string BuildMapperClassString(Item templateItem, string templateStructPrefix)
        {
            StringBuilder sb = new StringBuilder(string.Empty);
            string itemName = string.Empty;
            string fieldName = string.Empty;
            TemplateItem template = null;
            TemplateFieldItem templateField = null;
            List<TemplateFieldItem> allTemplateFields = null;
            Item fieldParentItem = null;

            ///build C# mapper class
            itemName = templateItem.Name.Trim().Replace(" ", "");
            sb.AppendLine("using Sitecore;");
            sb.AppendLine("using Sitecore.Data.Items;");
            sb.AppendLine("using System.Collections.Generic;");
            sb.AppendLine("using System.Web.Mvc;");
            sb.AppendLine();
            sb.AppendLine(" public class " + itemName + " : CustomItem ");
            sb.AppendLine(" {");///start class                
            sb.AppendLine("     public " + itemName + "(Item innerItem) : base(innerItem){}");

            template = MinionHelper.Databases.masterDb.GetTemplate(templateItem.ID);

            if (template != null)
            {
                ///get children of a template item i.e fields
                List<Item> itemFieldChildren = GetTemplateFields(templateItem, true, out allTemplateFields);

                if (itemFieldChildren != null && itemFieldChildren.Count > 0)
                {
                    templateStructPrefix = templateStructPrefix.Trim().TrimEnd('.');
                    templateStructPrefix = string.IsNullOrEmpty(templateStructPrefix) ? "References.Templates." : templateStructPrefix + ".";

                    ///class properties
                    foreach (Item fieldItem in itemFieldChildren)
                    {
                        templateField = allTemplateFields.Where(x => x.ID == fieldItem.ID).FirstOrDefault();

                        if (templateField != null)
                        {
                            fieldParentItem = fieldItem.Axes.GetAncestors().Where(x => x.TemplateID == MinionConstants.Templates.Template.ID).FirstOrDefault();
                            itemName = fieldParentItem != null ? fieldParentItem.Name : itemName;
                            itemName = itemName.Trim().Replace(" ", "");
                            fieldName = templateField.Name.Trim().Replace(" ", "");

                            switch (templateField.Type)
                            {
                                case MinionConstants.FieldTypes.Checkbox:
                                    sb.AppendLine();
                                    sb.AppendLine("     public bool " + fieldName);
                                    sb.AppendLine("     {");
                                    sb.AppendLine("         get");
                                    sb.AppendLine("         {");
                                    sb.AppendLine("             return InnerItem.Fields[" + templateStructPrefix + itemName + ".Fields." + fieldName + "].Value == \"1\";");
                                    sb.AppendLine("         }");
                                    sb.AppendLine("     }");
                                    break;
                                case MinionConstants.FieldTypes.Number:
                                case MinionConstants.FieldTypes.Integer:
                                    sb.AppendLine();
                                    sb.AppendLine("     public int " + fieldName);
                                    sb.AppendLine("     {");
                                    sb.AppendLine("         get");
                                    sb.AppendLine("         {");
                                    sb.AppendLine("             return MainUtil.GetInt(System.Convert.ToString(InnerItem.Fields[" + templateStructPrefix + itemName + ".Fields." + fieldName + "].Value), 0);");
                                    sb.AppendLine("         }");
                                    sb.AppendLine("     }");
                                    break;
                                case MinionConstants.FieldTypes.Date:
                                case MinionConstants.FieldTypes.Datetime:
                                    sb.AppendLine();
                                    sb.AppendLine("     public DateTime " + fieldName);
                                    sb.AppendLine("     {");
                                    sb.AppendLine("         get");
                                    sb.AppendLine("         {");
                                    sb.AppendLine("             return DateUtil.IsoDateToDateTime(System.Convert.ToString(InnerItem.Fields[" + templateStructPrefix + itemName + ".Fields." + fieldName + "].Value));");
                                    sb.AppendLine("         }");
                                    sb.AppendLine("     }");
                                    break;
                                case MinionConstants.FieldTypes.Image:
                                    sb.Append(GetIdPropertyString(templateStructPrefix, itemName, fieldName));
                                    sb.AppendLine();
                                    sb.AppendLine("     public string " + fieldName + "Url");
                                    sb.AppendLine("     {");
                                    sb.AppendLine("         get");
                                    sb.AppendLine("         {");
                                    sb.AppendLine("             return InnerItem.ImageUrl(" + templateStructPrefix + itemName + ".Fields." + fieldName + ");");
                                    sb.AppendLine("         }");
                                    sb.AppendLine("     }");

                                    sb.AppendLine();
                                    sb.AppendLine("     public string " + fieldName + "Alt");
                                    sb.AppendLine("     {");
                                    sb.AppendLine("         get");
                                    sb.AppendLine("         {");
                                    sb.AppendLine("             return InnerItem.ImageAlt(" + templateStructPrefix + itemName + ".Fields." + fieldName + ");");
                                    sb.AppendLine("         }");
                                    sb.AppendLine("     }");
                                    break;
                                case MinionConstants.FieldTypes.GeneralLink:
                                    sb.Append(GetIdPropertyString(templateStructPrefix, itemName, fieldName));
                                    sb.AppendLine();
                                    sb.AppendLine("     public string " + fieldName);
                                    sb.AppendLine("     {");
                                    sb.AppendLine("         get");
                                    sb.AppendLine("         {");
                                    sb.AppendLine("             return InnerItem.LinkFieldUrl(" + templateStructPrefix + itemName + ".Fields." + fieldName + ");");
                                    sb.AppendLine("         }");
                                    sb.AppendLine("     }");

                                    sb.AppendLine();
                                    sb.AppendLine("     public string " + fieldName + "TargetType");
                                    sb.AppendLine("     {");
                                    sb.AppendLine("         get");
                                    sb.AppendLine("         {");
                                    sb.AppendLine("             return InnerItem.LinkFieldTarget(" + templateStructPrefix + itemName + ".Fields." + fieldName + ");");
                                    sb.AppendLine("         }");
                                    sb.AppendLine("     }");
                                    break;
                                case MinionConstants.FieldTypes.SingleLineText:
                                case MinionConstants.FieldTypes.MultiLineText:
                                case MinionConstants.FieldTypes.RichText:
                                    sb.Append(GetIdPropertyString(templateStructPrefix, itemName, fieldName));
                                    sb.AppendLine();
                                    sb.AppendLine("     public MvcHtmlString " + fieldName);
                                    sb.AppendLine("     {");
                                    sb.AppendLine("         get");
                                    sb.AppendLine("         {");
                                    sb.AppendLine("             return new MvcHtmlString(InnerItem.Fields[" + templateStructPrefix + itemName + ".Fields." + fieldName + "].Value);");
                                    sb.AppendLine("         }");
                                    sb.AppendLine("     }");
                                    break;
                                case MinionConstants.FieldTypes.Droplink:
                                case MinionConstants.FieldTypes.Droptree:
                                    sb.AppendLine();
                                    sb.AppendLine("     public Item Selected" + fieldName + "Item");
                                    sb.AppendLine("     {");
                                    sb.AppendLine("         get");
                                    sb.AppendLine("         {");
                                    sb.AppendLine("             return SitecoreUtility.GetItem(InnerItem.Fields[" + templateStructPrefix + itemName + ".Fields." + fieldName + "].Value);");
                                    sb.AppendLine("         }");
                                    sb.AppendLine("     }");
                                    break;
                                case MinionConstants.FieldTypes.Multilist:
                                case MinionConstants.FieldTypes.MultilistWithSearch:
                                case MinionConstants.FieldTypes.Treelist:
                                case MinionConstants.FieldTypes.TreelistEx:
                                    sb.AppendLine();
                                    sb.AppendLine("     public List<object> " + fieldName + " { get; set; }");
                                    break;
                                default:
                                    sb.Append(GetIdPropertyString(templateStructPrefix, itemName, fieldName));
                                    break;
                            }
                        }
                    }
                }
            }

            sb.AppendLine(" }");///end class

            return sb.ToString();
        }

        /// <summary>
        /// get the fields of a template
        /// </summary>
        /// <param name="templateItem"></param>
        /// <param name="includeBaseTemplates"></param>
        /// <returns></returns>
        private static List<Item> GetTemplateFields(TemplateItem templateItem, bool includeBaseTemplates, out List<TemplateFieldItem> allTemplateFields)
        {
            allTemplateFields = new List<TemplateFieldItem>(); /// this is for all fields including std. template fields
            List<TemplateFieldItem> tempFields = new List<TemplateFieldItem>();
            List<Item> templateCustomFieldItems = null;
            List<Item> baseTemplateCustomFieldItems = null;
            List<Item> tempItems = null;

            if (templateItem != null)
            {
                templateCustomFieldItems = MinionHelper.GetItemsByTemplate(templateItem, MinionConstants.Templates.TemplateField.ID);
                allTemplateFields.AddRange(templateItem.Fields);

                if (includeBaseTemplates)
                {
                    if (templateCustomFieldItems == null)
                    {
                        templateCustomFieldItems = new List<Item>();
                    }

                    ///get all base templates of this template excluding 'standard template'
                    IEnumerable<TemplateItem> baseTemplates = templateItem.BaseTemplates.Where(x => x.ID != MinionConstants.Templates.StandardTemplate.ID);

                    ///add all  base template's fields to the fields list
                    if (baseTemplates != null && baseTemplates.Count() > 0)
                    {
                        foreach (TemplateItem tItem in baseTemplates)
                        {
                            tempItems = null;
                            tempFields = null;

                            ///get custom fields in this base template
                            baseTemplateCustomFieldItems = MinionHelper.GetItemsByTemplate(tItem, MinionConstants.Templates.TemplateField.ID);

                            if (baseTemplateCustomFieldItems != null && baseTemplateCustomFieldItems.Count > 0)
                            {
                                tempItems = baseTemplateCustomFieldItems.Except<Item>(templateCustomFieldItems).ToList();

                                if (tempItems != null && tempItems.Count > 0)
                                {
                                    templateCustomFieldItems.AddRange(tempItems);
                                }
                            }

                            tempFields = tItem.Fields.Except<TemplateFieldItem>(allTemplateFields).ToList();

                            if (tempFields != null && tempFields.Count > 0)
                            {
                                allTemplateFields.AddRange(tempFields);
                            }
                        }
                    }
                }
            }

            return templateCustomFieldItems;
        }

        private static string GetIdPropertyString(string templateStructPrefix, string itemName, string fieldName)
        {
            StringBuilder sb = new StringBuilder("");
            sb.AppendLine();
            sb.AppendLine("     public string " + fieldName + "Id");
            sb.AppendLine("     {");
            sb.AppendLine("         get");
            sb.AppendLine("         {");
            sb.AppendLine("             return " + templateStructPrefix + itemName + ".Fields." + fieldName + ".ToString();");
            sb.AppendLine("         }");
            sb.AppendLine("     }");
            return sb.ToString();
        }

        #endregion
    }
}
﻿namespace Basiscore.CmsAudit
{
    using Sitecore.Configuration;
    using Sitecore.Data;
    using Sitecore.Diagnostics;
    using System;
    using System.Collections.Generic;
    using System.Web;

    public class Configurations
    {
        public static readonly bool EnableCmsAuditLogging = Settings.GetSetting("EnableCmsAuditLogging") == Constants.True;
        public static readonly string CoreDbName = Settings.GetSetting("CoreDbName", "core");

        public List<ID> SpecifiedTemplateIds
        {
            get
            {
                List<ID> templateIds = new List<ID>();

                try
                {
                    if (HttpContext.Current.Session[Constants.Sessions.AUDIT_ITEM_TEMPLATES] != null)
                    {
                        templateIds = (List<ID>)HttpContext.Current.Session[Constants.Sessions.AUDIT_ITEM_TEMPLATES];
                    }
                    else 
                    {
                        string commaSeparatedTemplateIds = Settings.GetSetting("AuditOnlyItemsOfTheseTemplates", Constants.None);
                        commaSeparatedTemplateIds = commaSeparatedTemplateIds.TrimStart(Constants.Comma).TrimEnd(Constants.Comma).Trim();

                        if (string.IsNullOrWhiteSpace(commaSeparatedTemplateIds) || commaSeparatedTemplateIds.Equals(Constants.None, StringComparison.InvariantCultureIgnoreCase))
                        {
                            /// do nothing
                        }
                        else
                        {
                            string[] arrTemplateIds = commaSeparatedTemplateIds.Split(Constants.Comma);

                            if (arrTemplateIds != null && arrTemplateIds.Length > 0)
                            {
                                foreach (string templateId in arrTemplateIds)
                                {
                                    templateIds.Add(new ID(templateId.Trim()));
                                }                                
                            }
                        }
                    }

                    HttpContext.Current.Session[Constants.Sessions.AUDIT_ITEM_TEMPLATES] = templateIds;
                }
                catch (Exception ex)
                {
                    Log.Error(Constants.ModuleName, ex, this);
                }

                return templateIds;
            }
        }

        public List<string> SpecifiedStandardFieldNames
        {
            get
            {
                List<string> stdFieldNames = new List<string>();

                try
                {
                    if (HttpContext.Current.Session[Constants.Sessions.AUDIT_STANDARD_FIELDS] != null)
                    {
                        stdFieldNames = (List<string>)HttpContext.Current.Session[Constants.Sessions.AUDIT_STANDARD_FIELDS];
                    }
                    else
                    {
                        string commaSeparatedStandardFieldNames = Settings.GetSetting("IncludeTheseStandardFieldsInLog", Constants.None);
                        commaSeparatedStandardFieldNames = commaSeparatedStandardFieldNames.TrimStart(Constants.Comma).TrimEnd(Constants.Comma).Trim();

                        if (string.IsNullOrWhiteSpace(commaSeparatedStandardFieldNames) || commaSeparatedStandardFieldNames.Equals(Constants.None, StringComparison.InvariantCultureIgnoreCase))
                        {
                            /// do nothing
                        }
                        else
                        {
                            string[] arrFieldNames = commaSeparatedStandardFieldNames.Split(Constants.Comma);

                            if (arrFieldNames != null && arrFieldNames.Length > 0)
                            {
                                foreach (string fieldName in arrFieldNames)
                                {
                                    stdFieldNames.Add(fieldName.Trim());
                                }
                            }
                        }
                    }

                    HttpContext.Current.Session[Constants.Sessions.AUDIT_STANDARD_FIELDS] = stdFieldNames;
                }
                catch (Exception ex)
                {
                    Log.Error(Constants.ModuleName, ex, this);
                }

                return stdFieldNames;
            }
        }
    }
}
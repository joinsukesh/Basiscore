## OVERVIEW

I have developed this custom Sitecore module to audit the CMS Item changes. **CMS Audit**, in essence, subscribes to Sitecore events and logs the details into a custom table in the `core` database. The admin page allows you to easily track and monitor the history of item modifications. It provides a comprehensive record of item updates including a user friendly visualization of content differences before & after the save operaton.

## FEATURES
1. Logs details for all these item events - Create, Save, Rename, Version Add, Copy, Move, Duplicate, Clone, Order Change, Publish, Delete & Site Publish.
2. When an Item is saved, the field values before & after the save operation are captured.
3. Provides a visual representation of the content differences before & after the save operation.
4. The admin page displays these logs and allows the user to search by a date range, by Item Id, Username & Language.
5. Another admin page to purge the logs, if you want to.
   
## CONS
1. There could be a performance penalty on the CMS, when large number of users are using it, as each item event will be logged in the database.
2. Eventually, the number of records logged in the database would grow to a huge number. So, you need to periodically monitor the number of records and delete them if need be.
   
## INSTALLATION
1. [Download](https://github.com/joinsukesh/Downloads/blob/main/CMS%20Audit/CMSAudit_v1.zip) and install the package in your instance.

The package will install the following items in your instance:

**CMS (core):**
_/sitecore/client/Applications/Launchpad/PageSettings/Buttons/Basiscore_
_/sitecore/client/Applications/Launchpad/PageSettings/Buttons/Basiscore/CMS Audit_

**FILES:**
_/App_Config/Include/zzz.Basiscore/Basiscore.CmsAudit.config_
_/bin/Basiscore.CmsAudit.dll_
_/bin/HtmlDiff.dll_
_/sitecore/admin/cms-audit/ (includes pages & assets)_

2. If for any reason, you are unable to install the package in the instance, extract the content of the zip file and copy the files into the respective locations. The CMS core items are for the Launchpad Button. You can create your own and give this target URL - _/sitecore/admin/cms-audit/item-audit.aspx_

3. [Download](https://github.com/joinsukesh/Downloads/blob/main/CMS%20Audit/CmsAudit.sql) this file and execute these queries in the **core** database of your instance. 
These are the SQL queries to create the custom table, and the stored procedures to insert, fetch & delete the audit logs.

## CONFIGURATIONS
You can configure the module settings here - _/App_Config/Include/zzz.Basiscore/Basiscore.CmsAudit.config_
| Property  | Description |
| ------------- | ------------- |
| EnableCmsAuditLogging  | The value can be `true` or `false`. By default, it is set to `true`. If it is not `true`, then audit logs will not be inserted into the DB tables. Consider this only as a temporary way to disable logging, because the events in custom code will still be triggered, but logs will not be inserted into the table. To disable audit logging completely, comment the whole "events" & "pipelines" nodes in the config file.  |
| CoreDbName  | The value should be the name of the connection string of `core` database. By default, it is set to _core_. |
| MasterDbName  | The value should be the name of the connection string of `master` database. By default, it is set to _master_. |
| LogChangesMadeInDatabases  | Only changes made in the specified databases will be logged. Default values are 
_core,master_. |
| AuditOnlyItemsOfTheseTemplates  | Default value is _None_. If you want to insert audit logs for items of only certain templates, replace _None_ with comma separated Item IDs. This setting value should either be _None_ or have Template ID(s). |
| IncludeTheseStandardFieldsInLog  | The inserted audit logs will store field values of custom fields and the standard field names specified here. This setting value should either be _None_ or have exact standard field names separated by commas. |

## HOW TO USE
1. Once the pa


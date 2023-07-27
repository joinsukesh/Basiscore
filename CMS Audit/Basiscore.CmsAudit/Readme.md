## OVERVIEW

I have developed this custom Sitecore module which helps in auditing the CMS Item changes.   
**CMS Audit**, in essence, subscribes to Sitecore events and logs the details into a custom table in the `core` database.   
The admin page allows you to easily track and monitor the history of item modifications.   
It provides a comprehensive record of item updates, including a user friendly visualization of content differences before & after the save operaton.

## FEATURES
1. Logs details for all these item events - `Create`, `Save`, `Rename`, `Version Add`, `Copy`, `Move`, `Duplicate`, `Clone`, `Order Change`, `Publish`, `Delete` & `Site Publish`.
2. When an Item is saved, the field values before & after the save operation are captured.
3. Provides a visual representation of the content differences before & after the save operation.
4. The admin page displays these logs and allows the user to search by a date range, by Item Id, Username & Language.
5. Includes another admin page to purge the logs, if you want to.
   
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
### Basiscore.CmsAudit.config
You can configure the module settings here - _/App_Config/Include/zzz.Basiscore/Basiscore.CmsAudit.config_
| Property  | Description |
| ------------- | ------------- |
| EnableCmsAuditLogging  | The value can be `true` or `false`. By default, it is set to `true`. If it is not `true`, then audit logs will not be inserted into the DB tables. Consider this only as a temporary way to disable logging, because the events in custom code will still be triggered, but logs will not be inserted into the table. To disable audit logging completely, comment the whole "events" & "pipelines" nodes in the config file.  |
| CoreDbName  | The value should be the name of the connection string of `core` database. By default, it is set to _core_. |
| MasterDbName  | The value should be the name of the connection string of `master` database. By default, it is set to _master_. |
| LogChangesMadeInDatabases  | Only changes made in the specified databases will be logged. Default values are _core,master_. |
| AuditOnlyItemsOfTheseTemplates  | Default value is _None_. If you want to insert audit logs for items of only certain templates, replace _None_ with comma separated Item IDs. This setting value should either be _None_ or have Template ID(s). |
| IncludeTheseStandardFieldsInLog  | The inserted audit logs will store field values of custom fields and the standard field names specified here. This setting value should either be _None_ or have exact standard field names separated by commas. |

### CMS (master)
The following items keep getting updated frequently, even when CMS is refreshed or idle. This means, the _OnItemSaving_ method in the custom event handler will be triggered. The logic to exclude the audit logging on those "schedule" items is already handled in code.
However, as an extra measure, you can disable these items (if you are not using them), by setting the _end date_ to any passed date.
- _/sitecore/system/Settings/Email/Instance Tasks/Content Management Primary/Message Statistics/Today_
- _/sitecore/system/Tasks/Schedules/Content Testing/Suspend Corrupted Tests_
- _/sitecore/system/Tasks/Schedules/Content Testing/Try Finish Test_

## HOW TO USE
1. Login to your instance and make some item changes like, create, rename, modify the content, publish & delete.
2. Open the Launchpad and click the CMS Audit icon or browse this URL - _/sitecore/admin/cms-audit/item-audit.aspx_
![image](https://github.com/joinsukesh/Basiscore/assets/24619393/aa0da4e4-bc41-4cc7-a011-5feb6fc92ed0)

3. In the **Item Audit Logs** page, click on **Submit**. It should display all the item changes made in the selected date range.
![image](https://github.com/joinsukesh/Basiscore/assets/24619393/fdcd35ec-6554-4d94-9247-853b0898b747)

4. The **View** button will open a modal and display the item's content changes before & after the save operation. The first section will show the differences like this (if any). Then you can scroll down to see & compare the fields & values json data.
![image](https://github.com/joinsukesh/Basiscore/assets/24619393/2950d33e-7646-4117-83ad-cdaed517018e)

5. Go to the **Purge Logs** page. The table will show the number of records in the logs table (`Basiscore_CmsAudit_Items`). You can select the date range and click on **DELETE ITEM AUDIT LOGS** to delete all the logs in that date range. If you do not wish to have this page available, you can delete it here from your instance folder - _/sitecore/admin/cms-audit/urge-logs.aspx_

## NOTES
1. There will be one or more logs created depending on the Sitecore event. For example, Sitecore triggers both created & saved events when an item is created.
   
| Event  | Records Created in Table |
| ------------- | ------------- |
|Item Created|3 (1 for create & 2 for save)|
|Item Renamed|1|
|Item Renamed|1|
|Item Saved|1|
|Item Version Added|1|
|Item Duplicated|2 (1 for create & 1 for copy)|
|Item Copied|2 (1 for create & 1 for copy)|
|Item Cloned|5 (1 for create, 3 for save & 1 for clone)|
|Item Moved|1|
|Item Order Changed|1|
|Item Language Version Added|0|
|Item Published|1|
|Site Published|1|
|Item Deleted|1. But if an item is deleted with its subitems, then as many records as the number of subitems, will be inserted in the table.|

2. When an item is published through workflow, the `Published Subitems` propery in the `Comments` column, will be `true`.
3. If a user has selected `Danish` language in CMS and proceeds to publish the item, the `Language` property in the `Item Info` column may not be `Danish`. This is because the publish job operation is independent from the context language or the language selected by the user  in the Content Editor. So, cross verify this with the "Languages" in the `Comments` column.

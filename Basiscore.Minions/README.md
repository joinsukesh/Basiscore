## BASISCORE.MINIONS  
Minions are these gloating wisenheimer creatures, residing deep inside your project kernels.  
But, they might be of some help for a few tasks.  

## DESCRIPTION  
This project is a suite of Sitecore tools aimed to help developer's in some of their everyday tasks.  

## INSTALLATION  
1. Take a backup of the <b>/bin</b> folder, <b>/App_Config</b> folder, and the <b>Web.config</b>, from your instance's webroot.
2. Clone the solution.
3. The <b><i>/packages/Libraries</i></b> folder has the Sitecore & other DLLs used for this project (e.g.: Sitecore.Kernel etc). You can replace them with the ones from your instance webroot or leave them as is, as they will not be published.
4. Build the Solution and publish the project. It will publish the files to <i>C:\out\Basiscore.Minions</i>.  
5. Have a look at the files and copy them to your instance's webroot. 
Your instance may already have one or more DLLs e.g.: DocumentFormat.OpenXml.dll. Ignore those and copy the rest.  
5. Once the installation is done, browse this URL - <i>[your-instance-name]/sitecore/admin/minions/index.aspx</i>, to view the suite.

![image](https://user-images.githubusercontent.com/24619393/202977301-57a1a2a3-5965-4fea-b9ed-e2f087ba9c7e.png)

## HOW TO USE  
Choose a tool from the left navigation menu.  
Expand the "Instructions" section for each tool to understand what it does.  

![image](https://user-images.githubusercontent.com/24619393/202977641-de9987f3-e069-44f4-83c9-3452f550276e.png)

## FEATURES  
Currently, these are the tools available:  

### Items
1. <b>Import Excel Data</b> - Create/Update Sitecore items from data in an excel sheet.  
2. <b>Publish Items</b> - Perform a Smart Publish for bulk items, from Master to the target database(s), in one go. Choose from two input modes - Enter the paths of the items, or pick them from an existing Sitecore package.
### Fields
3. <b>Search Fields</b> - Find items that have the keyword as a raw value, in any of their fields. Get items and their field values for a field.
4. <b>Update Fields</b> - Replace the keyword in a field value. Update the field value.
### Packages
5. <b>Generate Package</b> - Quickly generate a Sitecore package by specifying the item paths.
6. <b>Install Package</b> - Install a Sitecore package that is uploaded in the instance.
### Renderings
7. <b>Add Renderings</b> - Add a rendering (with datasource) to multiple pages, in the master database.
8. <b>Remove Renderings</b> - Add a rendering (with datasource) to multiple pages, in the master database.
9. <b>Update Datasource</b> - Update a rendering's datasource for multiple pages, in the master database.
10. <b>Rendering Usage</b> - Find all the pages under a parent node, which have used a rendering.
### Misc
11. <b>Template Mapper</b> - Generate template references and its mapper class, that you can copy & use in your code.

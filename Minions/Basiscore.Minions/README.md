## OVERVIEW  
> Minions are these gloating wisenheimer creatures, residing deep inside your project kernels.  
But, they might be of some help for a few tasks.  

## DESCRIPTION  
This project is a suite of Sitecore tools aimed to help developers in some of their everyday tasks.  

## INSTALLATION  
1. Take a backup of the `/bin` folder, `/App_Config` folder, and the `Web.config`, from your instance's webroot.
2. Clone the Git solution.
3. The `/packages/Libraries` folder has the Sitecore & other DLLs used for this project (e.g.: Sitecore.Kernel etc). You can replace them with the ones from your instance webroot or leave them as is, as they will not be published.
4. Build the Solution and publish the project. It will publish the files to `C:\out\Basiscore.Minions`.  
5. Have a look at the files and copy them to your instance's webroot. 
Your instance may already have one or more DLLs e.g.: DocumentFormat.OpenXml.dll. Ignore those and copy the rest.  
5. Once the installation is done, browse this URL - `[your-instance-name]/sitecore/admin/minions/index.aspx`, to view the suite.

![image](https://user-images.githubusercontent.com/24619393/202977301-57a1a2a3-5965-4fea-b9ed-e2f087ba9c7e.png)

## HOW TO USE  
Choose a tool from the left navigation menu.  
Expand the `Instructions` section for each tool to understand what it does.  

![image](https://user-images.githubusercontent.com/24619393/202977641-de9987f3-e069-44f4-83c9-3452f550276e.png)

## FEATURES  
Currently, these are the tools available:  

#### Items

1. **Import Excel Data** - Create/Update Sitecore items from data in an excel sheet.  
2. **Publish Items** - Perform a Smart Publish for bulk items, from Master to the target database(s), in one go. Choose from two input modes - Enter the paths of the items, or pick them from an existing Sitecore package.
#### Fields
3. **Search Fields** - Find items that have the keyword as a raw value, in any of their fields. Get items and their field values for a field.
4. **Update Fields** - Replace the keyword in a field value. Update the field value.
#### Packages
5. **Generate Package** - Quickly generate a Sitecore package by specifying the item paths.
6. **Install Package** - Install a Sitecore package that is uploaded in the instance.
#### Renderings
7. **Add Renderings** - Add a rendering (with datasource) to multiple pages, in the master database.
8. **Remove Renderings** - Add a rendering (with datasource) to multiple pages, in the master database.
9. **Update Datasource** - Update a rendering's datasource for multiple pages, in the master database.
10. **Rendering Usage** - Find all the pages under a parent node, which have used a rendering.
#### Misc
11. **Template Mapper** - Generate template references and its mapper class, that you can copy & use in your code.

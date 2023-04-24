## OVERVIEW

Once the Sitecore components are developed & deployed for a project, developers will usually prepare a Word/PDF document for the content author, which has instructions on how to use the CMS, how to add content to a data source, etc.

This **Content Authoring Guide** (CAG) Sitecore module will create a simple & appealing web page template in your Sitecore instance, which serves the same purpose as the former, but with an interesting and user-friendly interface. 
This will be an in-house page, hosted in the CMS itself.  

The content for this CAG page will be picked up from the related Sitecore items. 

All that needs to be done by the developer is to fill the instructions in the respective items.

These instructions are displayed in an engaging webpage than an otherwise monotonous document.
As, the content is rendered from Sitecore items, 
- This makes it is easy for the developers to create/edit them as and when there is a change.
- Also, the articles will have a consistent format on the page. And with the carousel slide, the information could be more compelling.

It also has the nice Search feature to perform a quick search on the components listed in the page.
This page can be browsed right from the CMS itself instead of having a separate document to refer to all the time.

The CAG page can be opened from the Launch pad and looks like this: 
![image001](https://user-images.githubusercontent.com/24619393/203292880-37f129ac-ea33-4362-b895-a55060241d03.png)

## INSTALLATION

### Code
1.	Take a backup of the `/bin` folder, `/App_Config` folder, and the `Web.config`, from your instance's webroot.
2.	Clone the Git solution.
3.	The `/packages/Libraries` folder has the Sitecore & other DLLs used for this project (e.g.: Sitecore.Kernel etc). You can replace them with the ones from your instance webroot or leave them as is, as they will not be published.
4.	Build the Solution and publish the project. It will publish the files to `C:\out\Basiscore.ContentAuthoringGuide`.
5.	Have a look at the files and copy them to your instance's webroot. 

### Sitecore Items
You can choose either of these two ways to install the Sitecore items into your instance.

#### Method 1: Installing the package

Install the Sitecore package that is in the Solution- `Basiscore.ContentAuthoringGuide > CMS > Basiscore_ContentAuthoringGuide-v1.0.zip`  

#### Method 2: Using Sitecore CLI

If you have Sitecore CLI setup for your instance, then:

 **1.** Open terminal with administrator privilges and change the directory to the root folder of this solution.
 
 **2.** Execute this command to connect to your Sitecore instance:
 
        `dotnet sitecore login --authority https://[instance-identityserver] --cm https://[instance] --allow-write true`
        
 **3.** Then execute this comman to pull items from the disk to CMS
 
        `dotnet sitecore ser pull`
        

Any of the above steps will create the following items, in your instance:

**TEMPLATES:**

- /sitecore/templates/Basiscore/Content Authoring Guide/…
- /sitecore/templates/Branches/Basiscore/Content Authoring Guide/…

**MEDIA:**

/sitecore/media library/Basiscore/Content-Authoring-Guide/...

**DEMO CONTENT:**

/sitecore/content/Content Authoring Guide/…

**LAUNCH PAD BUTTON (core database):**

/sitecore/client/Applications/Launchpad/PageSettings/Buttons/Basiscore/Content Authoring Guide

This tool uses MVC Areas in the backend, which must be registered. So, do an IIS reset from the Internet Information Services (IIS) Manager. This will be a one-time step.

## VIEW THE DEMO GUIDE

Open the Launch Pad in your instance and click on the Content Authoring Guide icon. 

 ![image002](https://user-images.githubusercontent.com/24619393/203295548-c0de76bc-dbe7-425f-afc2-b04bee9532ed.png)


This will open a page with the URL - 
`[instance-name]/ContentAuthoringGuide/{2B2E25EC-1313-42F7-983F-5FDCA163A5CD}`
The ID in the URL is the Item ID of the Guide Root Item - `/sitecore/content/Content Authoring Guide`.

**NOTE:**

For the purpose of demonstration, the package also includes some sample content, which is created at - `/sitecore/content/Content Authoring Guide`.

If you like to have this node at some other location, move it there. Afterwards, update its path in the Link field of the launch pad button (**core** database), here -
`/sitecore/client/Applications/Launchpad/PageSettings/Buttons/Basiscore/Content Authoring Guide`.

![image003](https://user-images.githubusercontent.com/24619393/203295664-4c98e1f7-d21f-4f0b-9b41-1ba14fa1c3af.png)

The ID here should be the Item ID of the Guide Root Item that you have just created/moved.

## ADD CONTENT TO THE GUIDE

### Cover Page
 
 ![image004](https://user-images.githubusercontent.com/24619393/203295733-7683b1d1-ed82-4b00-b3f1-3ffb8becf4f6.png)

The cover page title, background image and other content can be populated in the respective items in this root item - `/sitecore/content/Content Authoring Guide`.

### Articles

Let’s assume that you want to impart instructions to the author about two topics: 
- How to upload an image in the CMS &
- How to add content for the Accordion component.

It would make sense to categorize these articles. In this case, we can have a **General** category, which will have instructions on all general topics, and then a **Components** category, which will have articles related to components.

1. Under the CAG root item (`/sitecore/content/Content Authoring Guide`), create two **Article** Folders using the template - `/sitecore/templates/Basiscore/Feature/Content Authoring Guide/Articles Folder`, and fill in the field values.

 ![image005](https://user-images.githubusercontent.com/24619393/203295828-6abd8fff-e59b-440a-a93d-d7d0b7141455.png)

You can also create Articles directly under the CAG root along with Article Folders. It all depends on how you would like the sidebar menu to be on the CAG page.

2. Now create the Article under General using the template - `/sitecore/templates/Basiscore/Content Authoring Guide/Article`, and fill in the field values.

 ![image006](https://user-images.githubusercontent.com/24619393/203295889-9c2ce51e-6605-4569-85a1-143b87514ed5.png)

3. It’s always a good idea to have images in your article that explain your instructions more clearly.
Upload the related images to `Media Library` first. Then create a Carousel Slide under the Article using the template - `/sitecore/templates/Basiscore/Content Authoring Guide/Carousel Slide`, and fill in the field values.

If you have added content in the `Description` field, it will show the image and the content side-by-side. If there is no content, then the image will be rendered in full width, inside the carousel.

Add as many such slides as needed for your article.

![image007](https://user-images.githubusercontent.com/24619393/203295975-719f7f8d-2249-4faf-81bf-6f7b6f6d7934.png)

 4. Now, similarly create another Article for the Accordion.

 ![image008](https://user-images.githubusercontent.com/24619393/203296057-1b7464e4-a2d7-4b06-98a0-4242e64f6c09.png)

5. Browse the CAG page and you can see that the content and menu are rendered using the Sitecore items that you have just created.

 ![image009](https://user-images.githubusercontent.com/24619393/203296123-138c30fe-c300-40e4-a6b0-783829b6fd84.png)



### Search

Authors can also use the search feature on the page to search for articles having the keyword.

 ![image010](https://user-images.githubusercontent.com/24619393/203296175-2c171668-b7b6-406c-9a9d-cc4234ed9f1d.png)

The keyword should have minimum 2 letters. 
The application will search for the keyword in the following fields and display the results:
- **Article** - Title, Description, Reference URL, Section header & description fields.
- **Carousel Slide** - Description.

## FOR MULTIPLE GUIDES

1. If you have multiple sites in your instance, you can create another Content Authoring Guide item for that site using the branch template - `/sitecore/templates/Branches/Basiscore/Content Authoring Guide/Guide Root`
2. To have its Launch Pad icon, create a new item in the **core** database under `/sitecore/client/Applications/Launchpad/PageSettings/Buttons`. 
The template should be `/sitecore/client/Applications/Launchpad/PageSettings/Templates/LaunchPad-Button`.
3. In its Link field, give the URL as `/ContentAuthoringGuide/<id of your new Guide Root item>`.

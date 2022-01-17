



# Kentico13CoreBaseline
Our Kentico 13 Baseline for MVC .Net Core 5.0 Site, the perfect starting point for your Kentico Xperience 13 Site to get up and running right away.

# Installation
Install a normal Kentico 13 Site, and hotfix it up to at least 13.0.57 (KX13 Refresh 3)

Also make sure to install .Net 5.0 and .Net Core 3.1 onto your solution (you can install the Hosting Bundles as well if you plan on hosting via IIS)

## Install NuGet Packages
On the Kentico Admin (WebApp/Mother) solution, install the following NuGet Packages

1. [RelationshipsExtended](https://www.nuget.org/packages/RelationshipsExtended/)
2. [PageBuilderContainers.Kentico](https://www.nuget.org/packages/PageBuilderContainers.Kentico/)
3. [XperienceCommunity.PageCustomDataControlExtender](https://github.com/wiredviews/xperience-page-custom-data-control-extender)

Optionally install
5. [HBS_CSVImport](https://www.nuget.org/packages/HBS_CSVImport/) (will be upgraded to 13 in near future)
6. [HBS.AutomaticGeneratedUserRoles.Kentico](https://www.nuget.org/packages/HBS.AutomaticGeneratedUserRoles.Kentico/) (may not be needed with new Authorization plugin already installed)

Make sure you have Visual Studio 2019 or higher, and the Visual Studio extension [Web Essentials 2019](https://marketplace.visualstudio.com/items?itemName=MadsKristensen.WebEssentials2019) is recommended.

## Upgrading / Hotfixing Admin
If you already had the Baseline for Admin, or are upgrading / hotfixing in the future, make sure to update the `Kentico.Xperience.Libraries` nuget package on the admin to the version your site is either on or hotfixing to.  The NuGet packages this Baseline uses inherits this nuget package, and **if you fail to update this package after you hotfix, your Admin solution will probably not work.**

# FRESH INSTALL
## Install Site Objects / Settings
When starting fresh, please perform the following operations in your Kentico Xperience Admin instance:

1. `Sites` - `Import Site or Object` and upload the [Baseline Site generic objects](Baseline_Generics.1.0.0.zip).  You can import these objects into your existing site.
2. Go to `Page Types` and on all `Generic.XXXX` page types, edit them and go to `Sites` and add them to your site.
3. Go to `Pages`, and...
   * Add a `Home` page to the root
   * Add a `Folder` called `MasterPage` to the root
   * Add a `Folder` called `Navigation` under `MasterPage` (Navigation items go here)
   * Add a `Header` called `Header` and a `Footer` called `Footer` under `MasterPage`
4. Go to `Settings` -> `URLs & SEO` and set the `Routing mode` to `Based on Content Tree` and the Default Homepage to your `Home` page. 

## Enable Webfarm

Kentico uses Webfarm to sync media file changes, event triggers, and more importantly, Cache dependency touches.  Please go to `Settings - Versioning & Synchronization - Web Farm` and set to `Automatic` (you can also set it to Manual if you wish).  By default, the Web farm names will be the server name (for your admin site) and the Server Name + "`_AutoExternalWeb`" for your MVC site. 

## Replace MVC Solution with Baseline
1. Remove the default MVC site folder and replace it with this repository
2. Copy `appsettings.json` and rename it `appsettings.Development.json`, update the empty fields with your connection string, Signature hash salt, and your Admin url.
3. Open the MVC Solution
4. Restore Nuget Packages (May have to run `Update-Package -reinstall` in nuget command prompt)
5. Update any nuget packages for custom tools (usually `Something.Kentico.Core`) installed if needed, however do **not** update non-custom Kentico Nuget Packages as this could break your solution if Kentico's code depends on certain versions.
6. Rebuild solution

If using IIS, there is also a `web.config`, you will have to update the aspNetCore processPath to point to the project's .exe file.  It is recommended however you use either `IIS Express` for your MVC Site or straight up `Kuberneties`

## Optional: Adding MetaData to Page Types
The Baseline site references the various `MetaData_XXX` fields in the DocumentCustomData of pages.  These are included in the `Base Inherited Page` type that you should inherit from.  However, if you do not wish to use the `Base Inherited Page` then you can add the fields yourself manually.

If you wish to add these fields manually, create `Field without database representation` on your page type, and mimic the `MetaData_xxx` fields you see on the `Base Inherited Page`.  The baseline defaults to the normal Page Meta Data within the Properties -> MetaData for all fields except the thumbnail image which isn't supported by default without the `MetaData_ThumbnailSmall` Document Custom Data.

**Versioning**: 
To enable versioning on the DocumentCustomData field (which is used for the Image MetaData in the baseline), please see the [Register VersionManager](https://github.com/wiredviews/xperience-page-custom-data-control-extender) step, which adds XML elements to your web.config


# UPGRADING FROM EXISTING BASELINE

## Upgrade From Kentico 12 Baseline
If you have a Baseline KX12 site and are going to KX13, it is recommended that after you upgrade your KX12 instance, that you start with the base KX13 Baseline site and just add in the customization / html.  The differences of MVC5 and .net 5 MVC (core) are pretty large.  Many elements are 1 to 1, but the structure requires a complete start over.

### Before you Upgrade
the Kentico12Baseline uses the [Dynamic Routing](https://github.com/KenticoDevTrev/DynamicRouting) Module, please read that module's upgrade instructions in order to preserve your URLs.  You will need to run some scripts to fix the Generic.Folder page type so it will properly be handled during upgrade.  Instructions and links on the Dynamic Routing Page.

## Upgrade from previous Kentico 13 Baseline

Upgrading from an existing Kentico Xperience 13 Core Baseline may prove a bit difficult as the entire repository was revamped.  Features wise and Admin/Database wise not much has changed (aside from how SEO meta data is stored/retrieved), however the whole project structure was revamped.  It may be easier if you want to upgrade your baseline to simply start with the new baseline and add in your features / custom code and views.

## MetaData to DocumentCustomData
In KX 12 MVC we used a tool for the SEO Meta Data that serialized / deserialized the MetaData into a long text field.  In the knew KX 13 baseline, we leverage the DocumentCustomData to assign Key-Values of the SEO Data.  If you are upgrading from KX12, please do the following:

1. Add [this conversion script](https://github.com/HBSTech/Kentico13CoreBaseline/blob/master/MetaDataToCustomData.zip) to your Kentico Admin solution
2. Include the files in your solution and build
3. Navigate to the `MetaDataToCustomData.aspx` page
4. BACKUP DATABASE (just in case)
5. Run the script
6. Remove the files from your solution once done.

# Baseline Information
Please see the [wiki](https://github.com/HBSTech/Kentico13CoreBaseline/wiki) for full details of features and tools.


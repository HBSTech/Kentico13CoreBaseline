# Kentico13CoreBaseline
Our Kentico 13 Baseline for MVC .Net Core 6.0 Site, the perfect starting point for your Kentico Xperience 13 Site to get up and running right away.

# Installation
Install a normal Kentico 13 Site, and hotfix it up to at least 13.0.62 (KX13 Refresh 3)

Also make sure to install .Net 6.0 and .Net Core 3.1 onto your solution (you can install the Hosting Bundles as well if you plan on hosting via IIS)

## Install NuGet Packages
On the Kentico Admin (WebApp/Mother) solution, install the following NuGet Packages

1. [RelationshipsExtended](https://www.nuget.org/packages/RelationshipsExtended/)
2. [PageBuilderContainers.Kentico](https://www.nuget.org/packages/PageBuilderContainers.Kentico/)
3. [XperienceCommunity.PageCustomDataControlExtender](https://github.com/wiredviews/xperience-page-custom-data-control-extender)

Optionally install
5. [HBS_CSVImport](https://www.nuget.org/packages/HBS_CSVImport/) (will be upgraded to 13 in near future)
6. [HBS.AutomaticGeneratedUserRoles.Kentico](https://www.nuget.org/packages/HBS.AutomaticGeneratedUserRoles.Kentico/) (may not be needed with new Authorization plugin already installed)

Make sure you have Visual Studio 2022 or higher, and the Visual Studio extensions [Web Compiler 2022+](https://marketplace.visualstudio.com/items?itemName=Failwyn.WebCompiler64), [WebPack task Runner](https://marketplace.visualstudio.com/items?itemName=MadsKristensen.WebPackTaskRunner), [Bundler & Minifier 2022+](https://marketplace.visualstudio.com/items?itemName=Failwyn.BundlerMinifier64), and optionally [NPM Task Runner](https://marketplace.visualstudio.com/items?itemName=MadsKristensen.NPMTaskRunner).

## Upgrading / Hotfixing Admin
If you already had the Baseline for Admin, or are upgrading / hotfixing in the future, make sure to update the `Kentico.Xperience.Libraries` nuget package on the admin to the version your site is either on or hotfixing to.  The NuGet packages this Baseline uses inherits this nuget package, and **if you fail to update this package after you hotfix, your Admin solution will probably not work.**

# Bug Fixes / Features Added
Bug fixes are mentioned here by date and MVC Version # (see MVC/MVC/MVC.csproj version #).  The commit history shows all changes.

**Version 1.2.3 (March 11, 2022)** [link 1.2.3](https://github.com/HBSTech/Kentico13CoreBaseline/commit/99457a8497fd35869b41ee2919006bdbd6cf98dd)
* Swapped the BundlerMinifier package with the Core Variant to prevent SCSS Build errors

**Version 1.2.1/1.2.2 (March 10, 2022)** [link 1.2.1](https://github.com/HBSTech/Kentico13CoreBaseline/commit/e4faa1ceee19c4ddeea7fe8870f9de10ba63d217) [link 1.2.2](https://github.com/HBSTech/Kentico13CoreBaseline/commit/a3f5f4e523a97f0c036deae046a64ee3169a50b8)
* Fixed one bug with NavigationRepsitory
* Removed redundent call in Startup.config
* Fixed BreadcrumbRepository.cs returning the root node, as well as not handling properly the Valid ClassName setting. [issue #9](https://github.com/HBSTech/Kentico13CoreBaseline/issues/9)

**Version 1.2.0 (MAJOR) (March 9, 2022)** [link](https://github.com/HBSTech/Kentico13CoreBaseline/pull/11)
* Hotfixed to 13.0.62 (includes fix to DocumentQuery.WithPageUrlPaths() that now renders custom EnsureUrls() obsolete)
* Replaced EnsureUrls() with WithPageUrlPaths() and removed PageRetrieverDocumentQueryExtension.cs)
* Fixed missing end parenthesis on NavigationRepository if declaring dynamic menu of certain type.
* Upgraded solutions to .net 6.0
* Added GlobalNamespace.cs files to solutions
* Added External Authentication options (Facebook, Google, Microsoft, Twitter)
* Added Two Factor Authentication (Email) option
* Added AuthenticationBuilder.ConfigureAuthentication(options) Extenstion method to configure external Auth settings
* Incorporated XperienceCommunity.WidgetFilter (removed UserWidgetProvider usage)

**Version 1.1.1 (Feb 11, 2022)**
* Fixed bug on CacheDependencyKeysBuilder.Object(string objectType, string name) which had !string.IsNullOrWhitespace instead of string.IsNullOrWhitespace, thus not adding the cache dependency key when called

**Version 1.1.0 (Feb 1, 2022)** [link](https://github.com/HBSTech/Kentico13CoreBaseline/commit/79d0b046743eff5edaefbb376f8ae41dc3747038)
* Added version # for bug tracking
* Fixed PageCategoryRetriever.GetCategoriesByIdentifiersAsync usage of query.GetEnumerableResultAsync to be query.ExecuteReaderAsync and loaded into a DataTable (otherwise the reader would close before data retrieved)
* Fixed PageCategoryRetriever.GetCategoriesByIdentifiersAsync changed _progressiveCache to be LoadAsync vs. Load.  It's invalid to call an Async method from within Load, must use LoadAsync

**Version 1.0.0 (MAJOR) (January 24, 2022)** [link] (https://github.com/HBSTech/Kentico13CoreBaseline/commit/a3e228b6b845c78d41ca4baddb85ec9ea54d7aa2)
* Added ToPageIdentity<TreeNode>() and PageIdentity<Type>() that passed properties in pageIdentity.Data
* Updated TabParentPageTemplate and TabParentViewComponent to show an optional leverage of typed identity and PageIdentityFactory conversion.

**Version 1.0.0 (January 19, 2022)**  [link](https://github.com/HBSTech/Kentico13CoreBaseline/commit/14189cbb4ab7356f0ba734c3286670c4beb8d619)
* Major revamp to Account system:
  * removed Public user fall back on GetUser methods (except GetCurrentUser)
  * Fixed where ModelState wasn't saving the actual model with it's properties, causing issues with Post-Redirect-GetCategoriesByIdentifiersAsync
  * Added IModelStateService to help store View Model to pass back and forth
  
* Added missing FluentValidation registration in AppStart
* Fixed namespace issues [Link1](https://github.com/HBSTech/Kentico13CoreBaseline/commit/6fe6afce8827d932e09f1cb15d50746949171861) [Link2](https://github.com/HBSTech/Kentico13CoreBaseline/commit/973f2a37aa9d4bdd079fc3dd9348dcbef3cdd999)

**Version 1.0.0 (January 18, 2022)**
* Fixed namespace issue [Link](https://github.com/HBSTech/Kentico13CoreBaseline/commit/f1ee7365298a030cb58e2907515c60c060b5396e)

**Version 1.0.0 (January 17, 2022)**
* Various fixes, including missing Site settings key dependencies for Account Urls [link](https://github.com/HBSTech/Kentico13CoreBaseline/commit/c71a1bd1dce7aebcb1e4027a7e7f55e028600fb5)

**Version 1.0.0 (January 12)** [link](https://github.com/HBSTech/Kentico13CoreBaseline/commit/97679a9674676a194066eedd89f8080160d157d8)
* Fixed typo on TreeNodeExtension that if the error on getting the Urls for PageIdentity fail, to use the fullPage instead of the page to retrieve.

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


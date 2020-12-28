


# Kentico13CoreBaseline
Our Kentico 13 Baseline for MVC .Net Core 5.0 Site, the perfect starting point for your Kentico 13 Site to get up and running right away.

***

# Installation
Install a normal Kentico 13 Site, and hotfix it up 13.0.5 (this is the minimum version that supports .Net Core 5.0)

## Install NuGet Packages
On the Kentico Admin (WebApp/Mother) solution, install the following NuGet Packages

1. [RelationshipsExtended](https://www.nuget.org/packages/RelationshipsExtended/)
1. [PageBuilderContainers.Kentico](https://www.nuget.org/packages/PageBuilderContainers.Kentico/)

Optionally install
1. [HBS_CSVImport](https://www.nuget.org/packages/HBS_CSVImport/) (will be upgraded to 13 in near future)
2. [HBS.AutomaticGeneratedUserRoles.Kentico](https://www.nuget.org/packages/HBS.AutomaticGeneratedUserRoles.Kentico/)

## Install Site Objects
In your Kentico Admin instance, go to `Sites` - `Import Site or Object` and upload the [Baseline Site](https://github.com/HBSTech/Kentico13CoreBaseline/blob/master/Kentico13CoreBaseline-AdminSiteImport.zip).  You can import these objects into an existing site or create the site from it.

## Enable Webfarm

Kentico uses Webfarm to sync media file changes, event triggers, and more importantly, Cache dependency touches.  Please go to `Settings - Versioning & Synchronization - Web Farm` and set to `Automatic` (you can also set it to Manual if you wish).  By default, the Web farm names will be the server name (for your admin site) and the Server Name + "`_AutoExternalWeb`" for your MVC site. 

## Replace MVC Solution with Baseline
1. Remove the default MVC site folder and replace it with this repository
2. Copy `appsettings.template` and rename it `appsettings.json`, update the empty fields with your connection string, Signature hash salt, and your Admin url.
3. Open the MVC Solution
4. Restore Nuget Packages (May have to run `Update-Package -reinstall` in nuget command prompt)
5. Update any nuget packages for custom tools (usually `Something.Kentico.Core`) installed if needed, however do **not** update non-custom Kentico Nuget Packages as this could break your solution if Kentico's code depends on certain versions.
6. Rebuild solution

***



Baseline Items
======================================================================

# Features
1. Bootstrap4 and jQuery included (with Bootstrap Layout Tool)
2. Header, Footer Configurations
3. Main Navigation with SubNav, MegaMenu, and Dynamic Menu capabilities
4. SEO Title/Thumbnail with OG Tag generation (uses Kentico's Page Meta Data page type feature, currently Image is not included so you do need to pass this yourself)
5. Breadcrumb and Side Navigation systems
6. User Management
7. Sitemap.xml Generation
8. Generic Pages
9. Tab Pages
10.Shareable Component Page with Widget 
11. Rich Text Editor, Image Widget, Static HTML Widgets, and a couple more
12. Widget Provider to configure what users see what widgets
14. MVCCaching for automatic Caching of IRepository and OutputCacheDependency injection
16. AutoMapper
17. production.GitIgnore for your projects

***
# Tool Explanations
Below I will now go through all the systems in the Baseline and explain how they operate.

### [Startup.cs](https://github.com/HBSTech/Kentico13CoreBaseline/blob/master/MVC/MVC/Startup.cs)

Your Startup.cs contains the starting point of your application, and it is where most of the core systems are activated. 

### [StartupConfig.cs](https://github.com/HBSTech/Kentico13CoreBaseline/blob/master/MVC/MVC/App_Start/StartupConfig.cs)

A simple static helper class that compartmentalizes some of the pipeline sections of the Startup.cs to make it easier to read.  Note however, you must be *very* careful adjusting the order things are initialized as you can very easily break your site by putting things out of order in your Core Pipeline.

### [RouteConfig.cs](https://github.com/HBSTech/Kentico13CoreBaseline/blob/master/MVC/MVC/App_Start/RouteConfig.cs)

The Routing priority contains Kentico's Route handler and it's routing, MVC Route attribute priority, Errors, Sitemap, Default Route. 

### [bundleconfig.json](https://github.com/HBSTech/Kentico13CoreBaseline/blob/master/MVC/MVC/bundleconfig.json)

Contains the bundling of CSS and Javascript for certain areas. This is how .Net Core bundles css / javascript

### [AutoMapperMaps.cs](https://github.com/HBSTech/Kentico13CoreBaseline/blob/master/MVC/MVC/App_Start/AutoMapperMaps.cs)

This contains the AutoMapper mapping configuration, that you can leverage with the IMapper interface to convert one object to another.

### Dependency Injection

.Net Core has built in Dependency Injection (DI) and is utilized throughout the solution.  Any implementations configured will be automatically available on your Controllers, Views, Components, really everywhere.

As part of MVC Caching however, the default DI did not support the methods needed to perform the caching operations needed, so Castle Windsor and AutoFac are part of the default installation.  These are only used for `IRepository` and `IService` inherited interfaces and their implementations.  You do not need to leverage these in your own dependency injection, other than to know they are there.  Please see the [MVC Caching git repo](https://github.com/KenticoDevTrev/MVCCaching) for more information.

You can also see it registering `AutoMapper`.

### [Views/_ViewImports.cshtml](https://github.com/HBSTech/Kentico13CoreBaseline/blob/master/MVC/MVC/Views/_ViewImports.cshtml)

.Net Core allows for _ViewImports.cshtml to be placed at any level in your Views folder to define namespaces, taghelpers, and interface injections that all sibling and child views inherit from.  This file contains the ones needed for the baseline.

The Master Template ([\_Layout.cshtml](https://github.com/HBSTech/Kentico13CoreBaseline/blob/master/MVC/MVC/Views/Shared/_Layout.cshtml))
----------------------------------------------------------------------------------------------------------------------------------------------

In Portal Engine (or those familiar with Web Forms), there was the concept of the Master Page.  In MVC, this is instead the Layout of a particular view.  The Layout will contain the wrapper around your page, normally the Header and Footer.  This section we will dive into the Layout and its elements.

### Styles/Javascript

If you take a look at our [\_Layout.cshtml](https://github.com/HBSTech/Kentico13CoreBaseline/blob/master/MVC/MVC/Views/Shared/_Layout.cshtml), you're going to see a lot of things going on to add in the Header/Footer Custom Elements, Styles, and Javascript, as well as Page Builder functionality.  
Going from top to bottom...

* `<environment include="Development">` and `<environment exclude="Development">` - These are tag helpers that render content differently on Development (debugging) vs. Live, we use these to pull in the bundled css/js for live, but show the normal files on dev for easier debugging.
* `<page-builder-mode include="Edit">` and `<page-builder-mode exclude="Edit">` - These are tag helpers that Sean G Wright created to render content differently on Page Builder Edit mode vs. Live Mode.
*   `<script src="~/js/scripts/jquery-3.5.1.min.js"></script>` - Since jQuery is often needs to be available right away, I also add this to the header.
*   `<script src="~/js/bundles/HeaderJS.min.js"></script>` - Any additional header JavaScript, keep in mind you should keep header JavaScript to a minimum as it blocks load.
*   `<link href="~/css/bundles/HeaderStyles.min.css" rel="stylesheet" />` - Any Header CSS style sheets
*   `@RenderSection("head", required: false)` - This is where each view that uses this shared layout can add header elements, such as the meta tags and OG:Tags, or schema.org json+lt scripts
*   `<page-builder-styles />` - If you are on a page with Page Builder (in edit mode), this will render the Styles needed for the page builder to operate.  This also is used with `<link href="~/css/EditMode.min.css" rel="stylesheet" />` which is a custom css you may need for things to look okay in edit mode (if you need to tweak margins for example)

And at the bottom of the Layout:
* `<script src="~/js/bundles/jqueryval.min.js"></script>`- jQuery Validator for MVC
*   `<script src="~/js/bundles/jquery-unobtrusive-ajax.min.js"></script>` - Ajax validator
* `<page-builder-scripts />` - Renders Page builder javascript.
* `<script src="~/js/scripts/MyComponentsConfiguration.js"></script>` Custom Froala Editor configuration file (shared toolbar).
* `<script src="~/js/scripts/updatableFormHelper.js"></script>` and `<script src="~/js/scripts/inputmask.js"></script>` if not on edit mode as the Forms often need these files to still operate.  They may no longer be needed, i wasn't able to confirm.
* `<script src="~/js/Custom.js"></script>` or `<script src="~/js/bundles/FooterJS.min.js"></script>` - The custom javascript (later one is bundled for non-development mode).
* `<script src="~/js/bootstrap/popper.min.js"></script>` and `<script src="~/js/bootstrap/bootstrap.min.js"></script>` - Bootstrap JS and menu system.
*   `@RenderSection("bottom", required: false)` - This is where each view that uses this shared layout can add footer elements, such as additional JavaScript libraries needed.

### Header and Footer Content

When it comes to the Header and Footer, you have two options available.  You can hard code the content, since it often does not change frequently, however any updates to the will have to be done by you in the future, and you lose some of the Page Builder personalization features. 

The other option is to give the editors a header and/or footer page with Page Builder zones, and then pull that content into your normal \_Layout using the [Partial Widget Page](https://github.com/KenticoDevTrev/PartialWidgetPage).  This is how you leverage the Partial Widget Page:

#### 1\. Generic.Header and Generic.Footer

The first piece is the page types themselves.  We have a special `[Generic.Header](https://github.com/HBSTech/Kentico13CoreBaseline/blob/master/MVC/MVC/Models/PageTypes/Header.cs)` and `[Generic.Footer](https://github.com/HBSTech/Kentico13CoreBaseline/blob/master/MVC/MVC/Models/PageTypes/Footer.cs)` page type that will give the users the Page Builder items on the content tree to edit the content.

#### 2\. Headerless and Footerless \_Layouts

The next piece is a clone of the normal [\_Layout.cshtml](https://github.com/HBSTech/Kentico13CoreBaseline/blob/master/MVC/MVC/Views/Shared/_Layout.cshtml) is required for the Edit mode of these page types.  We have a [\_Layout-_NoHeader.cshtml](https://github.com/HBSTech/Kentico13CoreBaseline/blob/master/MVC/MVC/Views/Shared/_Layout-NoHeader.cshtml) and [\_layout-_NoFooter.cshtml](https://github.com/HBSTech/Kentico13CoreBaseline/blob/master/MVC/MVC/Views/Shared/_Layout-NoFooter.cshtml), and as the names indicate, they are just the normal \_Layout.cshtml except they do not render the header and footer (and the \_Layout-NoHeader.cshtml's  `RenderBody()` is in the header section).  The reason we do this is because while editing, we still want all of our CSS and Javascript to render, but we don't want the header/footer to render while we are trying to edit it, otherwise you end up with an infinite loop as it tries to render the header with the header.

#### 3\. Special Views

For each page type, we have a view to render it, and what is key is in the layout declaration, we are using the `PWPHelper.LayoutIfEditMode` method, which means that the content around it will only render if it's being edited by the CMS.  Otherwise, it will only render the view itself (which is just the PageBuilder content).  In this way, when we pull the content into the header/footer areas, it is just that area's HTML.  Here’s the [Header](https://github.com/HBSTech/Kentico13CoreBaseline/blob/master/MVC/MVC/Views/Shared/Components/PartialHeader/Default.cshtml) and Here’s the [Footer](https://github.com/HBSTech/Kentico13CoreBaseline/blob/master/MVC/MVC/Views/Shared/Components/PartialFooter/Default.cshtml).

#### 4\. Routing

Since the rendering of these sections are very simple, we are going to just add two simple Controllers and routes for the [Header](https://github.com/HBSTech/Kentico13CoreBaseline/blob/master/MVC/MVC/Models/PageTypes/Header.cs) and [Footer](https://github.com/HBSTech/Kentico13CoreBaseline/blob/master/MVC/MVC/Models/PageTypes/Footer.cs).  These render out a View that only contains the Layout Toggle and a View Component call (`<vc:partial-header/>` or `<vc:partial-footer/>`).

Lastly, on the actual Layout, we use the Partial Widget Page Tag Helper to alter the rendering context and display:

``` csharp
<inlinewidgetpage documentid="@PWPHelper.GetDocumentIDByNode("/Masterpage/Header")" initialize-document-prior="true">
     <vc:partial-header />
</inlinewidgetpage>
        ...
<inlinewidgetpage documentid="@PWPHelper.GetDocumentIDByNode("/Masterpage/Footer")" initialize-document-prior="true">
     <vc:partial-footer />
</inlinewidgetpage>
```

### Navigation

Navigation systems usually fall into one of two categories: Manual or based on the Content Tree.

Manual navigation tends to be more flexible because many clients want specific control over what items show up on the navigation, especially considering how most navigation bars can only fit so many items before wrapping. 

Content Tree navigation was easier to maintain, you add a page, and instantly it was on the menu, and with Kentico's Page type Feature of [navigation](https://docs.xperience.io/developing-websites/building-website-navigation), you can use this again if you wish to build a .

My navigation system is a hybrid.  It is based on Navigation elements on the content tree, that can be either Manual or Automatic, and contain Manual or Dynamic children, or can be a Mega Menu, all based on the configuration.

Having this pre-made for you should take a lot of the development time off your shoulders, so let me explain this system for you.

#### Navigation Page Type

The Navigation Page Type is your manual navigation item.  It has multiple configurations, but the largest of which is if you wish the item itself to be `Manual` (you fill in the link, title, etc.), or `Automatic` (select a page and pull in its `PageMetaData` / Document Name and relative link automatically). 

The next item is in the Additional Configuration if it is a `Mega Menu`.  If this is true, then whatever content you place in the Page Builder (Page tab), it will pull it into the transformation. 

The last section, the `Dynamic` Section, allows the children to be Dynamically generated, this is like a repeater so you should be familiar with selecting your `Path`, `Page Types`, and other settings.  `NodeLevel, NodeOrder` for the `Order by` will mimic the tree structure.

Lastly, if you add `Node Categories` to the navigation items, you can filter what Navigation Elements are pulled in by the category code name (see the [INavigationRepository.GetNavItems()](https://github.com/HBSTech/Kentico13CoreBaseline/blob/master/MVC/MVC/RepositoryLibrary/Interfaces/INavigationRepository.cs)).  This can be useful if you have some elements that you wish to display on the Mobile navigation only vs. the Desktop, you can assign different categories to them and display.

#### [INavigationRepository](https://github.com/HBSTech/Kentico13CoreBaseline/blob/master/MVC/MVC/RepositoryLibrary/Interfaces/INavigationRepository.cs)

The `[INavigationRepository](https://github.com/HBSTech/Kentico13CoreBaseline/blob/master/MVC/MVC/RepositoryLibrary/Interfaces/INavigationRepository.cs)` is the interface that does all the heavy lifting, this is used in the `[HeaderController](https://github.com/HBSTech/Kentico13CoreBaseline/blob/master/MVC/MVC/Controllers/Structure/HeaderController.cs)` to render the navigation but be aware you can use this for other things.  It has methods to convert any list of Nodes into a `[HierarchyTreeNode](https://github.com/HBSTech/Kentico13CoreBaseline/blob/master/MVC/MVC/Models/Generic/HierarchyTreeNode.cs)`, and has logic for getting Side Navigations (more on those later).

#### Header Views

The last piece is the actual rendering of the navigation.  Included are the basic building blocks that you can adapt to your template. These are done using 
* [MainNavigation/Default.cshtml](https://github.com/HBSTech/Kentico13CoreBaseline/blob/master/MVC/MVC/Views/Shared/Components/MainNavigation/Default.cshtml) - The container for your navigation
*   [RenderNavigationItem.cshtml](https://github.com/HBSTech/Kentico13CoreBaseline/blob/master/MVC/MVC/Views/Shared/Navigation/RenderNavigationItem.cshtml) - Rendering for each type of navigation item, be it single, drop down, or mega menu.  This is the "top level" navigation
*   [RenderNavigationDropdownItem.cshtml](https://github.com/HBSTech/Kentico13CoreBaseline/blob/master/MVC/MVC/Views/Shared/Navigation/RenderNavigationDropdownItem.cshtml) - 2nd and beyond level rendering.

#### Navigation-less Layout, Navigation View, Dynamic Route Tag for Mega Menus

Just like the Header / Footer Content, the Mega Menu system uses the Partial Widget Page to allow the user to create WYSIWYG content on the Page tab of the navigation item and pull that into the mega menu itself.   It uses a [\_Layout-Noheader.cshtml](https://github.com/HBSTech/Kentico13CoreBaseline/blob/master/MVC/MVC/Views/Shared/_Layout-NoHeader.cshtml), a [Generic_Navigation.cshtml](https://github.com/HBSTech/Kentico13CoreBaseline/blob/master/MVC/MVC/Views/Shared/PageTypes/Generic_Navigation.cshtml) view (no need for a routing attribute as Kentico looks to this spot if none is defined).  The navigation is cached through the `[INavigationRepository](https://github.com/HBSTech/Kentico13CoreBaseline/blob/master/MVC/MVC/RepositoryLibrary/Interfaces/INavigationRepository.cs)` ([in the Implementation of it](https://github.com/HBSTech/Kentico13CoreBaseline/blob/master/MVC/MVC/RepositoryLibrary/Implementation/KenticoNavigationRepository.cs)) system.

#### Custom Cache Clear Event Hook

Lastly since Navigation items may be generated dynamically, a page a navigation item is pointing may update and not trigger the navigation cache to clear.  There is a [custom event hook](https://github.com/HBSTech/Kentico13CoreBaseline/blob/master/MVC/MVC/Library/Generic_LoaderModule.cs) that we do both on the Documents, their children if dynamic, and page categories.

***

### MetaData and Title

For rendering MetaData and Title, since Kentico re-introduced the Page Meta Data page type feature, we leverage this.  This is done through the custom view component `PageMetaData`, and all it requires is you pass the treenode to it, and optionally the Image for the og:image tag.

I also have a breadcrumbs-json page component that you can leverage to include that breadcrumb JSON-LD:

```csharp
@section head {
    <vc:page-meta-data page="@Model.Page" image-url="" />
    <vc:breadcrumbs-json include-default-breadcrumb="true" nodeid="@Model.Page.NodeID"/>
}
```
***

### Page Content

Lastly, the actual Page Content by the `@RenderBody()` tag in the various Layouts.

Generic Pages
-------------

At this point the only last thing to discuss on Page structure is adding new Pages / Page Types.  You can use Kentico’s [Content Tree Routing](https://docs.xperience.io/developing-websites/implementing-routing/content-tree-based-routing).  The Generic Page does not have a controller and just uses a specifically located view in order to render.

***

Secondary Navigation
--------------------

As a sample, on the [Generic_GenericPage.cshtml](https://github.com/HBSTech/Kentico13CoreBaseline/blob/master/MVC/MVC/Views/Shared/PageTypes/Generic_GenericPage.cshtml), I demonstrate showing a Secondary navigation using the `<vc:secondary-navigation />` View Component.  

Keep in mind that using the aforementioned `[INavigationRepository](https://github.com/HBSTech/Kentico13CoreBaseline/blob/master/MVC/MVC/RepositoryLibrary/Interfaces/INavigationRepository.cs)`, it's quite easy to mimic my system and create additional navigation styling to your liking.  The `GetSecondaryNavItems` takes a path and some other configurations, you can also use the `GetAncestorPath` method to show a side navigation from an ancestor of the current page.  The rendering views ([SecondaryNavigation/Default.cshtml](https://github.com/HBSTech/Kentico13CoreBaseline/blob/master/MVC/MVC/Views/Shared/Components/SecondaryNavigation/Default.cshtml), [SecondaryNavigationItem.cshtml](https://github.com/HBSTech/Kentico13CoreBaseline/blob/master/MVC/MVC/Views/Shared/Navigation/SecondaryNavigationItem.cshtml) and [SecondaryNavigationDropdownItem.cshtml](https://github.com/HBSTech/Kentico13CoreBaseline/blob/master/MVC/MVC/Views/Shared/Navigation/SecondaryNavigationDropdownItem.cshtml)) operate the same as the main navigation.

### Breadcrumbs

You also have a `Breadcrumb` view component that is demonstrated on the same [Generic_GenericPage.cshtml](https://github.com/HBSTech/Kentico13CoreBaseline/blob/master/MVC/MVC/Views/Shared/PageTypes/Generic_GenericPage.cshtml).  This one is much simpler as really a breadcrumb should be the current page, onwards up.  You just call the `<vc:breadcrumbs include-default-breadcrumb="true" nodeid="@Model.Page.NodeID"/>` view-component.  There is also a `DefaultBreadcrumb` you can configure (default uses two localization strings), and you can configure what page types should be included in a set by adding a settings key `BreadcrumbPageTypes` and have it a delimited list of class names.  Default includes all types.  The rendering of the View is found [here](https://github.com/HBSTech/Kentico13CoreBaseline/blob/master/MVC/MVC/Views/Shared/Components/Breadcrumbs/Default.cshtml).

There is also a `Breadcrumb JSON+LD` rendering that you can include in the header for extra SEO, again demonstrated in the [Generic_GenericPage.cshtml](https://github.com/HBSTech/Kentico13CoreBaseline/blob/master/MVC/MVC/Views/Shared/PageTypes/Generic_GenericPage.cshtml)

***

Contact Us
----------

For the Contact Us page, I just used the normal Generic Page and the Form Page Builder Widget that comes with Kentico…so nothing special to show there!

***

[Sitemap](https://github.com/HBSTech/Kentico13CoreBaseline/blob/master/MVC/MVC/Controllers/Administrative/SiteMapController.cs)
---------------------------------------------------------------------------------------------------------------------------

The Sitemap system is relatively simple, you use the `ISiteMapRepository` to get `SitemapNodes` from your page content, or manually create them, and then once you have a list of `SitemapNodes`, you call the `GetSitemapXml` and it will do create the markup and set the proper output and encoding.  The [RouteConfig.cs](https://github.com/HBSTech/Kentico13CoreBaseline/blob/master/MVC/MVC/App_Start/RouteConfig.cs) has two mappings that that allow `sitemap.xml` or `googlesitemap.xml` to render.  If running on IIS, you *may* also need to set in your web.config the system.webServer modules node to runAllManagedModulesForAllRequests

    <modules runAllManagedModulesForAllRequests="true">

[Robots.txt](https://github.com/HBSTech/Kentico13CoreBaseline/blob/master/MVC/MVC/wwwroot/robots.txt)
-----------------------------------------------------------------------------------------

For the `Robots.txt` file, you can just create one, there is no need for really any MVC logic unless you, for some reason, want the `Robots.txt` to be dynamic.  Then you can follow a similar pattern to the Sitemap.

***

User Management
---------------

One final piece many sites need is User Management.  I have included an [AccountController](https://github.com/HBSTech/Kentico13CoreBaseline/blob/master/MVC/MVC/Controllers/Administrative/AccountController.cs) and related views that allow for [Sign In](https://github.com/HBSTech/Kentico13CoreBaseline/blob/master/MVC/MVC/Views/Account/SignIn.cshtml), `Sign Out`, [Register](https://github.com/HBSTech/Kentico13CoreBaseline/blob/master/MVC/MVC/Views/Account/Register.cshtml) (with [Email Verification](https://github.com/HBSTech/Kentico13CoreBaseline/blob/master/MVC/MVC/Views/Account/CheckYourEmail.cshtml)), [Change Password](https://github.com/HBSTech/Kentico13CoreBaseline/blob/master/MVC/MVC/Views/Account/ResetPassword.cshtml), and [Reset Forgotten Password](https://github.com/HBSTech/Kentico13CoreBaseline/blob/master/MVC/MVC/Views/Account/ForgottenPasswordReset.cshtml) (Email password reset link).


I also included the Dancing Goat variety which you can swap out if you wish, otherwise it's not really used.
***

Code Systems
============

Now to briefly touch on various Code Systems this Baseline must help optimize and speed up development.

Dependency Injection
------------------------------

This was mentioned prior in this article.  Any class in your assembly that inherits the `IRepository` or `IService` will automatically be enabled for Dependency Injection, and any constructor that has the variables `string cultureName` and `bool lastVersionEnabled` will have those values set automatically.  Also, `IMapper` and `IDynamicRouteHelper` are included in the Automatic factoring. 

MVC Caching with IRepository
----------------------------

For Interfaces that retrieve data and inherit `IRepository`, you should follow this pattern when creating your interfaces:

1.  Any logic that retrieves data should be an Interface that inherits `IRepository`, any logic that simply manipulates or performs some action should be an Interface that inherits `IService`
2.  Then create your Implementation of these `Interfaces`, for Repositories, leverage the `MVCCaching` attributes.  Any call made through dependency injection that have `MVCCaching` will automatically cache with the proper dependencies, and the dependency keys will be added to the output cache.
3.  This may require a `Helper` interface so your calls within the implementation are also done through dependency injection.  For example, [KenticoRoleRepository](https://github.com/HBSTech/Kentico13CoreBaseline/blob/master/MVC/MVC/RepositoryLibrary/Implementation/KenticoRoleRepository.cs) (which implements [IRoleRepository](https://github.com/HBSTech/Kentico13CoreBaseline/blob/master/MVC/MVC/RepositoryLibrary/Interfaces/IRoleRepository.cs)) has a helper [IKenticoRoleRepositoryHelper](https://github.com/HBSTech/Kentico13CoreBaseline/blob/master/MVC/MVC/RepositoryLibrary/Interfaces/IKenticoRoleRepositoryHelper.cs) for some of its calls, so it can call it through an `IRepository` interface.

This will ensure that your code is automatically cached.  Definitely recommend reading the [MVCCaching Documentation](https://github.com/KenticoDevTrev/MVCCaching/blob/master/Documentation.md) for all of it’s nuances.  
 

Render Caching / Output Caching
-----------------------------------------

.Net Core comes with a standard `<cache>` tag helper, but Kentico is still in processes of leveraging a custom cache tag helper to incorporate kentico's cache dependencies. 

They are also looking into output caching as this is not part of native .Net Core (and rather hard to implement!)  So for now, there is only data level caching.  Luckily even without that, .Net core is super fast.

AutoMapper
----------

`AutoMapper` is a common tool to Map one model to another.  This is often used when mapping Data models to View Models.  You can include the `IMapper` interface in your constructor, and then use its `Map<TDestination>(SourceObject)`.  You can configure this and add your own mappings in the [AutoMapperMaps.cs](https://github.com/HBSTech/Kentico13CoreBaseline/blob/master/MVC/MVC/App_Start/AutoMapperMaps.cs).

Take it from here
==========================================

At this point you have the starting site structure.  Just take your website HTML / CSS / Javascript and start implementing it, adjust the views to match your navigation, add in your media files, etc.  There may be some specific widgets you will want to create, just follow [Kentico’s Documentation](https://docs.xperience.io/developing-websites/developing-xperience-applications-using-asp-net-core/page-builder-development-in-asp-net-core/developing-page-builder-widgets-in-asp-net-core), or specific [Page Templates with properties](https://docs.xperience.io/developing-websites/developing-xperience-applications-using-asp-net-core/page-builder-development-in-asp-net-core/developing-page-templates-in-asp-net-core).  


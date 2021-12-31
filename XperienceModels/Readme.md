# XperienceModels
This class should contain all of your Kentico Xperience Page Types / Custom Tables / Form Info/Info Provider classes.

This can also contain any SHARED (Admin + MVC) items such as Global Event Hooks.

This Library should *ONLY* reference `Kentico.Xperience.Libraries` and *SHOULD NOT* reference any admin /LIB drivers, nor the Kentico.Xperience.AspNet packages

This package should primarily be leveraged by the Site.Library project to properly implement interfaces from the Site.Models, but will also be referenced by the main MVC site for things such as Page Type and Routing declarations.


## Upgrading from Previous Baselines
If you are upgrading from a previous version of the Baseline, here's some things to take into consideration.

### Generic Page to Basic Page
In order to be able to replace "Generic" with some other prefix, I created a Generic.BasicPage vs. a Generic.GenericPage.  If you use Generic.GenericPage types, you will need to...
1. Adjust the Features/BasicPage/BasicPagePageTemplate.cs to be Generic.GenericPage for the template identity and use the Generic.GenericPage for the classname.
1. Adjust Features/BasicPage/BasicPagePage.cshtml to use the GenericPage class
1. Re-Add the GenericPage.cs file in XperienceModels -> PageTypes/Generic and remove the BasicPage.cs page type

### Applying Page Templates
The new baseline now uses PTVC (Page Template => View Controller) design, where as the previous used Page Type to Controller routing, thus the pages did not have a page template (or it was the empty one).

Please use the following script to set the page templates for the page types affected:

** TO DO **


## Refactoring "Generic" to something else
Generic is the default namespace for all items used in this baseline solution.  If you wish to use your own prefix, simply do the following:
1. Open the Baseline MVC Solution
1. Find/Replace "Generic" with "YourPrefix" on the ENTIRE Solution (use case sensitive find/replace)
1. Find/Replace "System.Collections.YourPrefix" with "System.Collection.Generic" on the ENTIRE Solution (use case sensitive find/replace)
1. In the XperienceModels -> Page Types, rename of the folder "Generic" to whatever your prefix is as well.
1. In the BreadcrumbRepository.cs, adjust the default breadcrumb resource strings to whatever you wish.
1. In the Kentico Admin, recreate the Generic Page Types (it is possible to just change the Namespace in the admin and regenerate the code, but the tables will still have Generic_ as the prefix)
1. On each page type, go to Code and replace the code files
1. Rebuild both the admin and MVC Solutions

Note: You should use caution doing this if you are upgrading from an existing Baseline System, as the Widget Codenames have been 'Generic.ImageWidget', 'Generic.ShareableContentWidget' in the past, so you should probably adjust the ImageWidget.cs and ShareableContentWidget.cs identities back to Generic so you don't break any pages that use those widgets.


## Baseline Structure
The Baseline system does have a couple paths it is expecting, unless you modify the _Layout.cshtml (and _Layout-NoFooter.cshtml and _Layout-NoHeader.cshtml), to replicate, create a tree structure as below under the root

* MasterPage (Generic.Folder)
	* Footer (Generic.Footer)
	* Header (Generic.Header)
	* Navigation (Generic.Folder)
		* [add Generic.Navigation here]


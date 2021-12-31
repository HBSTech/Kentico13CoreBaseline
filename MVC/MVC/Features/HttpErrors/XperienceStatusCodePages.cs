/*
 * The baseline has basic Error pages enabled, however Bizstream has a pretty sweet Errors Status code system that you can 
 * leverage to get Page Builder controlled errors.
 * https://www.bizstream.com/blog/october-2021/part-iii-bizstream-xperience-statuscodepages-pack
 * To Install:
 * 1. On this MVC Project, install the Nuget Package BizStream.Kentico.Xperience.AspNetCore.StatusCodePages
 * 2. in the App_Start/RouteConfig.cs, remove the HttpErrors route
 * 3. in the App_Start/StartupConfig.cs -> RegisterDotNetCoreConfigurationsAndKentico, comment out the app.UseStatusCodePagesWithReExecute("/error/{0}"); and uncomment the app.UseXperienceStatusCodePages();
 * 4. Uncomment out the 2 lines on this file to include the assembly
 * 5. On the Kentico Admin, intsall the nuget package BizStream.Kentico.Xperience.Adminstration.StatusCodePages
 * 
 * You should now be able to add the Status pages to your content tree.  Be careful that only stable elements go on them though so you don't end up with an infinite loop on 500 server error pages
 * */
//using BizStream.Kentico.Xperience.AspNetCore.StatusCodePages;
//[assembly: RegisterStatusCodePageRoute]
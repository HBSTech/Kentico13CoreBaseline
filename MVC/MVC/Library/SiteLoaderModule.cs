using CMS;
using CMS.DataEngine;
using CMS.DocumentEngine;
using CMS.DocumentEngine.Types.Generic;
using CMS.Helpers;
using Generic.Library;
using Kentico.Forms.Web.Mvc;
using System;

[assembly: RegisterModule(typeof(SiteLoaderModule))]

namespace Generic.Library
{
    public class SiteLoaderModule : Module
    {
        // Module class constructor, the system registers the module under the name "CustomInit"
        public SiteLoaderModule()
            : base("SiteLoaderModule")
        {
        }

        // Contains initialization code that is executed when the application starts
        protected override void OnInit()
        {
            base.OnInit();

            // Clear navigation either if Navigation Category touched
            TreeCategoryInfo.TYPEINFO.Events.Insert.After += CategoryNavigationCacheClear;
            TreeCategoryInfo.TYPEINFO.Events.Update.After += CategoryNavigationCacheClear;
            TreeCategoryInfo.TYPEINFO.Events.Delete.Before += CategoryNavigationCacheClear;

            // Or clear navigation if page is updated that is attached to a navigation item
            DocumentEvents.Update.After += DocumentNavigationCacheClear;
        }

        private void GetConfiguration_Execute(object sender, GetFormFieldRenderingConfigurationEventArgs e)
        {
            if (e.FormComponent.GetBizFormComponentContext().FormInfo.FormName.Equals("Contact"))
            {
                switch (e.FormComponent.Name.ToLower())
                {
                    case "phone":
                        e.FormComponent.BaseProperties.Placeholder = e.FormComponent.BaseProperties.Label;
                        //e.Configuration.EditorHtmlAttributes.Add("placeholder", "Phone");
                        break;
                }
                if (e.FormComponent.BaseProperties.Required)
                {
                    e.Configuration.EditorHtmlAttributes.Add("required", "required");
                    e.FormComponent.BaseProperties.Placeholder += "*";
                }
            }
        }

        private void DocumentNavigationCacheClear(object sender, DocumentEventArgs e)
        {
            try
            {
                if (DocumentHelper.GetDocuments<Navigation>()
                    .WhereEquals("NavigationPageNodeGuid", e.Node.NodeGUID)
                    .Columns("NodeID")
                    .Count > 0)
                {
                    CacheHelper.EnsureKey("CustomNavigationClearKey", DateTime.Now);
                    CacheHelper.TouchKey("CustomNavigationClearKey");
                }
            }
            catch (Exception) { }
        }

        private void CategoryNavigationCacheClear(object sender, ObjectEventArgs e)
        {
            try
            {
                TreeCategoryInfo Category = (TreeCategoryInfo)e.Object;

                // If a Navigation page was the one who was touched
                if (DocumentHelper.GetDocuments<Navigation>()
                    .WhereEquals("NodeID", Category.NodeID)
                    .Columns("NodeID")
                    .Count > 0)
                {
                    CacheHelper.EnsureKey("CustomNavigationClearKey", DateTime.Now);
                    CacheHelper.TouchKey("CustomNavigationClearKey");
                }
            }
            catch (Exception) { }
        }
    }
}
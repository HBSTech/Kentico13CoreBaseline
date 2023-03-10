using CMS;
using CMS.DataEngine;
using Navigation.Library;
using NavigationPageType = CMS.DocumentEngine.Types.Generic.Navigation;
[assembly: RegisterModule(typeof(NavigationLoaderModule))]

namespace Navigation.Library
{
    public class NavigationLoaderModule : Module
    {
        // Module class constructor, the system registers the module under the name "CustomInit"
        public NavigationLoaderModule()
            : base("NavigationLoaderModule")
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


        private void DocumentNavigationCacheClear(object? sender, DocumentEventArgs e)
        {
            try
            {
                if (DocumentHelper.GetDocuments<NavigationPageType>()
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

        private void CategoryNavigationCacheClear(object? sender, ObjectEventArgs e)
        {
            try
            {
                TreeCategoryInfo Category = (TreeCategoryInfo)e.Object;

                // If a Navigation page was the one who was touched
                if (DocumentHelper.GetDocuments<NavigationPageType>()
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
using CMS.DataEngine;
using CMS.Helpers;
using CMS.SiteProvider;
using Generic.Models;
using Generic.Repositories.Helpers.Interfaces;
using Generic.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using MVCCaching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Generic.Repositories.Implementations
{
    public class KenticoBreadcrumbRepository : IBreadcrumbRepository
    {
        private IKenticoBreadcrumbRepositoryHelper _Helper;
        private IServiceProvider _services;
        private readonly IUrlResolver UrlResolver;

        public KenticoBreadcrumbRepository(IRepoContext repoContext, IKenticoBreadcrumbRepositoryHelper Helper, IServiceProvider services, IUrlResolver urlResolver)
        {
            _Helper = Helper;
            _services = services;
            UrlResolver = urlResolver;
        }

        public BreadcrumbJsonLD BreadcrumbsToJsonLD(IEnumerable<Breadcrumb> Breadcrumbs, bool ExcludeFirst = true)
        {
            return BreadcrumbsToJsonLDAsync(Breadcrumbs, ExcludeFirst).Result;
        }

        public async Task<BreadcrumbJsonLD> BreadcrumbsToJsonLDAsync(IEnumerable<Breadcrumb> Breadcrumbs, bool ExcludeFirst = true)
        {
            var itemListElement = new List<ItemListElementJsonLD>();
            int Position = 0;
            foreach (Breadcrumb breadcrumb in (ExcludeFirst ? Breadcrumbs.Skip(1) : Breadcrumbs))
            {
                Position++;
                itemListElement.Add(new ItemListElementJsonLD()
                {
                    position = Position,
                    Name = breadcrumb.LinkText,
                    Item = UrlResolver.GetAbsoluteUrl(breadcrumb.LinkUrl)
                });
            }
            
            return new BreadcrumbJsonLD()
            {
                itemListElement = itemListElement
            };
        }

        [CacheDependency("node|##SITENAME##|/|childnodes")]
        public List<Breadcrumb> GetBreadcrumbs(int PageIdentifier, bool IncludeDefaultBreadcrumb = true)
        {
            return GetBreadcrumbsAsync(PageIdentifier, IncludeDefaultBreadcrumb).Result;
        }

        /// <summary>
        /// Gets the Breadcrumbs of the given page.
        /// </summary>
        /// <param name="PageIdentifier"></param>
        /// <param name="TopLevelBreadcrumb"></param>
        /// <returns></returns>
        [CacheDependency("node|##SITENAME##|/|childnodes")]
        public async Task<List<Breadcrumb>> GetBreadcrumbsAsync(int PageIdentifier, bool IncludeDefaultBreadcrumb = true)
        {
            string[] ValidClassNames = SettingsKeyInfoProvider.GetValue(new SettingsKeyName("BreadcrumbPageTypes", new SiteInfoIdentifier(SiteContext.CurrentSiteID))).ToLower().Split(";,|".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            List<Breadcrumb> Breadcrumbs = new List<Breadcrumb>();

            // Get the current Page and then loop through ancestors
            var Page = _Helper.GetBreadcrumbNode(PageIdentifier);
            bool IsCurrentPage = true;
            while (Page != null && !Page.ClassName.Equals("CMS.Root", StringComparison.InvariantCultureIgnoreCase))
            {
                if (ValidClassNames.Length == 0 || ValidClassNames.Contains(Page.ClassName.ToLower()))
                {
                    Breadcrumbs.Add(_Helper.PageToBreadcrumb(Page, IsCurrentPage));
                }
                Page = _Helper.GetBreadcrumbNode(Page.NodeParentID);
                IsCurrentPage = false;
            }

            // Add given Top Level Breadcrumb if provided
            if (IncludeDefaultBreadcrumb)
            {
                Breadcrumbs.Add(_services.GetService<IBreadcrumbRepository>().GetDefaultBreadcrumb());
            }
            // Reverse breadcrumb order
            Breadcrumbs.Reverse();
            return Breadcrumbs;
        }

        [CacheDependency("cms.resourcestring|byname|generic.default.breadcrumbtext")]
        [CacheDependency("cms.resourcestring|byname|generic.default.breadcrumburl")]
        public Breadcrumb GetDefaultBreadcrumb()
        {
            return GetDefaultBreadcrumbAsync().Result;
        }

        [CacheDependency("cms.resourcestring|byname|generic.default.breadcrumbtext")]
        [CacheDependency("cms.resourcestring|byname|generic.default.breadcrumburl")]
        public async Task<Breadcrumb> GetDefaultBreadcrumbAsync()
        {
            return new Breadcrumb()
            {
                LinkText = ResHelper.LocalizeExpression("generic.default.breadcrumbtext"),
                LinkUrl = ResHelper.LocalizeExpression("generic.default.breadcrumburl"),
            };
        }
    }
}
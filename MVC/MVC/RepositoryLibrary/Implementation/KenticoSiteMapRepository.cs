using CMS.DocumentEngine;
using CMS.Helpers;
using CMS.Localization;
using CMS.SiteProvider;
using Generic.Models;
using Generic.Repositories.Helpers.Interfaces;
using Generic.Repositories.Interfaces;
using Kentico.Content.Web.Mvc;
using Microsoft.AspNetCore.Mvc;
using MVCCaching;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Generic.Repositories.Implementations
{
    public class KenticoSiteMapRepository : ISiteMapRepository
    {
        private readonly IPageDataContextRetriever _dataContextRetriever;
        private readonly IKenticoSiteMapRepositoryHelper _Helper;
        private readonly IPageUrlRetriever _pageUrlRetriever;
        private readonly IGeneralDocumentRepository _generalDocumentRepository;

        public KenticoSiteMapRepository(IRepoContext repoContext, 
            [FromServices] IPageDataContextRetriever dataContextRetriever, 
            IKenticoSiteMapRepositoryHelper Helper,
            [FromServices] IPageUrlRetriever pageUrlRetriever,
            IGeneralDocumentRepository generalDocumentRepository)
        {
            _dataContextRetriever = dataContextRetriever;
            _Helper = Helper;
            _pageUrlRetriever = pageUrlRetriever;
            _generalDocumentRepository = generalDocumentRepository;
        }

        [DoNotCache]
        public IEnumerable<SitemapNode> GetSiteMapUrlSet(SiteMapOptions Options)
        {
            // Clean up
            Options.Path = DataHelper.GetNotEmpty(Options.Path, "/").Replace("%", "");
            Options.SiteName = DataHelper.GetNotEmpty(Options.SiteName, SiteContext.CurrentSiteName);
            List<SitemapNode> Nodes = new List<SitemapNode>();

            if (Options.ClassNames != null && Options.ClassNames.Count() > 0)
            {
                foreach (string ClassName in Options.ClassNames)
                {
                    if (string.IsNullOrWhiteSpace(Options.UrlColumnName))
                    {
                        Nodes.AddRange(_Helper.GetSiteMapUrlSetForClass(Options.Path, ClassName, Options.SiteName, Options));
                    }
                    else
                    {
                        // Since it's not the specific node, but the page found at that url that we need, we will first get the urls, then cache on getting those items.
                        Nodes.AddRange(GetSiteMapUrlSetForClassWithUrlColumn(Options.Path, ClassName, Options.SiteName, Options));
                    }
                }
            }
            else
            {
                Nodes.AddRange(_Helper.GetSiteMapUrlSetForAllClass(Options.Path, Options.SiteName, Options));
            }

            // Clean up, remove any that are not a URL
            Nodes.RemoveAll(x => !Uri.IsWellFormedUriString(x.Url, UriKind.Absolute));
            return Nodes;
        }

        /// <summary>
        /// Gets the SitemapNodes, looking up the page they point to automatically to get the accurate Document last modified.
        /// </summary>
        /// <param name="Path">The parent Nodealiaspath</param>
        /// <param name="ClassName">The Class Name to query</param>
        /// <param name="SiteName">The SiteName</param>
        /// <param name="Options">The Options</param>
        /// <returns>The SitemapNodes</returns>
        private IEnumerable<SitemapNode> GetSiteMapUrlSetForClassWithUrlColumn(string Path, string ClassName, string SiteName, SiteMapOptions Options)
        {
            var DocumentQuery = _Helper.GetDocumentQuery(Path, Options, ClassName);
            List<SitemapNode> SiteMapItems = new List<SitemapNode>();
            foreach (var Page in DocumentQuery.TypedResult)
            {

                string Culture = !string.IsNullOrWhiteSpace(Options.CultureCode) ? Options.CultureCode : LocalizationContext.CurrentCulture.CultureCode;
                var RelativeUrl = Page.GetStringValue(Options.UrlColumnName, _pageUrlRetriever.Retrieve(Page.NodeAliasPath, Culture, SiteName).RelativePath);


                // Try to find page by NodeAliasPath
                var ActualPage = (TreeNode)_generalDocumentRepository.GetDocumentByPath(RelativeUrl, SiteName, Columns: new string[] { "DocumentModifiedWhen", "NodeID", "DocumentCulture" });
                if (ActualPage != null)
                {
                    SiteMapItems.Add(ConvertToSiteMapUrl(RelativeUrl, SiteName, Page.DocumentModifiedWhen));
                }
                else
                {
                    SiteMapItems.Add(ConvertToSiteMapUrl(RelativeUrl, SiteName, null));
                }
            }
            return SiteMapItems;
        }

        /// <summary>
        /// Converts the realtive Url and possible Datetime into an SitemapNode with an absolute Url
        /// </summary>
        /// <param name="RelativeURL">The Relative Url</param>
        /// <param name="SiteName">The SiteName</param>
        /// <param name="ModifiedLast">The last modified date</param>
        /// <returns>The SitemapNode</returns>
        private SitemapNode ConvertToSiteMapUrl(string RelativeURL, string SiteName, DateTime? ModifiedLast)
        {
            string Url = URLHelper.GetAbsoluteUrl(RelativeURL, RequestContext.CurrentDomain);
            SitemapNode SiteMapItem = new SitemapNode(Url);
            if (ModifiedLast.HasValue)
            {
                SiteMapItem.LastModificationDate = ModifiedLast;
            }
            return SiteMapItem;
        }
    }
}
using CMS.Helpers;
using CMS.Membership;
using CMS.Search;
using CMS.WebAnalytics;
using Generic.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Xml;

namespace Generic.Controllers
{
    public class SearchController : Controller
    {
        private readonly IPagesActivityLogger mPagesActivityLogger;

        public SearchController(IPagesActivityLogger pagesActivityLogger)
        {
            mPagesActivityLogger = pagesActivityLogger;
        }
        // GET: Search
        [Route("/Search")]
        public ActionResult Index(string SearchValue = null)
        {
            SearchValue = ValidationHelper.GetString(SearchValue, "");
            SearchViewModel Model = new SearchViewModel()
            {
                SearchValue = SearchValue
            };
            if (!string.IsNullOrWhiteSpace(SearchValue))
            {
                var searchParameters = SearchParameters.PrepareForPages(SearchValue, new[] { "SiteSearch_Crawler", "SiteSearch_Pages" }, 1, 100, MembershipContext.AuthenticatedUser);
                var Search = SearchHelper.Search(searchParameters);

                // Special logic to handle Component - Page Metadata descriptions, will use this if present, otherwise uses content.
                foreach (var item in Search.Items)
                {
                    if (item.Data.GetStringValue("Component_PageMetadata", "").IndexOf("<Description>", StringComparison.InvariantCultureIgnoreCase) != -1)
                    {
                        XmlDocument MetaData = new XmlDocument();
                        MetaData.LoadXml(item.Data.GetStringValue("Component_PageMetadata", ""));
                        string PageDescription = MetaData.SelectSingleNode("//Description").InnerText;
                        // If we have the page metadata description, use it over content.
                        item.Content = !string.IsNullOrWhiteSpace(PageDescription) ? PageDescription : item.Content;
                    }
                }

                Model.SearchItems = Search.Items;

                mPagesActivityLogger.LogInternalSearch(SearchValue);
            }
            return View(Model);
        }

    }
}
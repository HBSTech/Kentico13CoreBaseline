using CMS.DocumentEngine.Types.Generic;
using Generic.Models;
using Generic.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Generic.Controllers.Administrative
{
    public class SiteMapController : Controller
    {

        private ISiteMapRepository mSiteMapRepo;
        public SiteMapController(ISiteMapRepository SiteMapRepo)
        {
            mSiteMapRepo = SiteMapRepo;
        }

        // GET: SiteMap
        [HttpGet]
        public ActionResult Index()
        {
            List<SitemapNode> Nodes = new List<SitemapNode>();
            // This site uses a custom navigation type, so we will base the Sitemap off of that.
                Nodes.AddRange(mSiteMapRepo.GetSiteMapUrlSet(new SiteMapOptions()
                {
                    Path = "/MasterPage/Navigation",
                    ClassNames = new string[] { Navigation.CLASS_NAME },
                    UrlColumnName = "NavigationLinkUrl",
                    CacheItemName = "SiteMap"
                })
            );
            // Add custom SitemapNodes here if you wish
            Nodes.AddRange(mSiteMapRepo.GetSiteMapUrlSet(new SiteMapOptions()
            {
                Path = "/%",
                ClassNames = new string[] { GenericPage.CLASS_NAME },
                CacheItemName = "SiteMapPages"
            }));
            // Now render manually, sadly the SimpleMVCSitemap disables output cache somehow
            return Content(GetSitemapXml(Nodes), "text/xml", Encoding.UTF8);
        }

        /// <summary>
        /// Formats the Nodes into the proper XML
        /// </summary>
        /// <param name="Nodes"></param>
        /// <returns></returns>
        private string GetSitemapXml(IEnumerable<SitemapNode> Nodes)
        {
                return $"<urlset xmlns=\"http://www.sitemaps.org/schemas/sitemap/0.9\">{string.Join("\n", Nodes.Select(x => SitemapNodeToXmlString(x)))}</urlset>";
        }

        /// <summary>
        /// Formats the Node into the proper XML
        /// </summary>
        /// <param name="Node"></param>
        /// <returns></returns>
        private string SitemapNodeToXmlString(SitemapNode Node)
        {
            string changeFreq = string.Empty;
            if (Node.ChangeFrequency.HasValue) {
                switch (Node.ChangeFrequency.Value)
                {
                    case ChangeFrequency.Always:
                        changeFreq = "always";
                        break;
                    case ChangeFrequency.Daily:
                        changeFreq = "daily";
                        break;
                    case ChangeFrequency.Hourly:
                        changeFreq = "hourly";
                        break;
                    case ChangeFrequency.Monthly:
                        changeFreq = "monthly";
                        break;
                    case ChangeFrequency.Never:
                        changeFreq = "never";
                        break;
                    case ChangeFrequency.Weekly:
                        changeFreq = "weekly";
                        break;
                    case ChangeFrequency.Yearly:
                        changeFreq = "yearly";
                        break;
                }
            }

            return string.Format("<url>{0}{1}{2}{3}</url>",
                $"<loc>{Node.Url}</loc>",
                Node.LastModificationDate.HasValue ? $"<lastmod>{Node.LastModificationDate.Value.ToString("yyyy-MM-ddTHH:mm:sszzz")}</lastmod>" : "",
                !string.IsNullOrWhiteSpace(changeFreq) ? $"<changefreq>{changeFreq}</changefreq>" : "",
                Node.Priority.HasValue ? $"<priority>{Node.Priority.Value}</priority>" : ""
                );
        }
    }
}
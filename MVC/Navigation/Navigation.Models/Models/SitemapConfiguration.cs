namespace Navigation.Models
{
    public class SitemapConfiguration
    {
        public SitemapConfiguration()
        {

        }

        public Dictionary<string, IEnumerable<SiteMapOptions>> SiteNameToConfigurations { get; set; } = new Dictionary<string, IEnumerable<SiteMapOptions>>();

        public void AddSitemapConfiguration(string sitename, IEnumerable<SiteMapOptions> options)
        {
            sitename = sitename.ToLower();
            if(!SiteNameToConfigurations.TryAdd(sitename, options))
            {
                SiteNameToConfigurations[sitename] = SiteNameToConfigurations[sitename].Union(options);
            }
        }
    }


}

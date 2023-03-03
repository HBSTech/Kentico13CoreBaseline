namespace Navigation.Models
{
    public class SitemapNode
    {
        public SitemapNode(string url)
        {
            Url = url;
        }

        public string Url { get; set; }
        public Maybe<DateTime> LastModificationDate { get; set; }
        public Maybe<ChangeFrequency> ChangeFrequency { get; set; }
        public Maybe<decimal> Priority { get; set; }

    }

    //
    // Summary:
    //     Change frequency of the linked document
    public enum ChangeFrequency
    {
        //
        // Summary:
        //     The value "always" should be used to describe documents that change each time
        //     they are accessed.
        Always = 0,
        //
        // Summary:
        //     Hourly change
        Hourly = 1,
        //
        // Summary:
        //     Daily change
        Daily = 2,
        //
        // Summary:
        //     Weekly change
        Weekly = 3,
        //
        // Summary:
        //     Monthly change
        Monthly = 4,
        //
        // Summary:
        //     Yearly change
        Yearly = 5,
        //
        // Summary:
        //     The value "never" should be used to describe archived URLs.
        Never = 6
    }
}

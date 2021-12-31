using System;
namespace Generic.Models
{
    public class SitemapNode
    {
        public string Url { get; set; }
        public DateTime? LastModificationDate { get; set; }
        public ChangeFrequency? ChangeFrequency { get; set; }
        public decimal? Priority { get; set; }

        public SitemapNode()
        {
           
        }
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

namespace TabbedPages.Models
{
    public class TabItem
    {
        public TabItem(string name, string localizedName, int documentID)
        {
            Name = name;
            LocalizedName = localizedName;
            DocumentID = documentID;
        }

        public string Name { get; set; }

        /// <summary>
        /// Localized Name of the Tab, same as Name unless a resource string was provided in the tab.
        /// </summary>
        public string LocalizedName { get; set; }
        public int DocumentID { get; set; }
    }
}

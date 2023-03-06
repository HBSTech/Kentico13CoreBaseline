namespace TabbedPages.Models
{
    public class TabItem
    {
        public TabItem(string name, int documentID)
        {
            Name = name;
            DocumentID = documentID;
        }

        public string Name { get; set; }
        public int DocumentID { get; set; }
    }
}

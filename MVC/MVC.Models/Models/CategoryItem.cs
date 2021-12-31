using System;

namespace Generic.Models
{
    public class CategoryItem
    {
        public int CategoryID { get; set; }
        public Guid CategoryGuid { get; set; }
        public string CategoryName { get; set; }
        public string CategoryDisplayName { get; set; }
    }
}

namespace Core.Models
{
    public class CategoryItem : IObjectIdentifiable
    {
        public CategoryItem(int categoryID, Guid categoryGuid, string categoryName, int categoryParentID, string categoryDisplayName)
        {
            CategoryID = categoryID;
            CategoryGuid = categoryGuid;
            CategoryName = categoryName;
            CategoryParentID = categoryParentID;
            CategoryDisplayName = categoryDisplayName;
        }

        public int CategoryID { get; set; }
        public int CategoryParentID { get; set; }
        public Guid CategoryGuid { get; set; }
        public string CategoryName { get; set; }

        public string CategoryDisplayName { get; set; }
        public Maybe<string> CategoryDescription { get; set; }
      
        public static CategoryItem UnfoundCategoryItem()
        {
            return new CategoryItem(0, Guid.Empty, "NoCategoryFound", 0, "No Category Found");
        }

        public ObjectIdentity ToObjectIdentity()
        {
            return new ObjectIdentity()
            {
                Id = CategoryID,
                Guid = CategoryGuid,
                CodeName = CategoryName
            };
        }
    }
}

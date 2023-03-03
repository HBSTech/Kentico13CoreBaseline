namespace Core.Models
{
    public class CategoryItem : IObjectIdentifiable
    {
        public CategoryItem(int categoryID, Guid categoryGuid, string categoryName, int categoryParentID)
        {
            CategoryID = categoryID;
            CategoryGuid = categoryGuid;
            CategoryName = categoryName;
            CategoryParentID = categoryParentID;
        }

        public int CategoryID { get; set; }
        public int CategoryParentID { get; set; }
        public Guid CategoryGuid { get; set; }
        public string CategoryName { get; set; }
        public Dictionary<string, Maybe<string>> CategoryDescriptions { get; set; } = new Dictionary<string, Maybe<string>>();
        public Dictionary<string, string> CategoryDisplayNames { get; set; } = new Dictionary<string, string>();

        public string CategoryDisplayName
        {
            get
            {
                return CategoryDisplayNames.Keys.Any() ? CategoryDisplayNames[CategoryDisplayNames.Keys.First()] : "";
            }
            set
            {
                if (!CategoryDisplayNames.Keys.Any())
                {
                    CategoryDisplayNames.Add("en-US", value);
                }
            }
        }

        public Maybe<string> CategoryDescription
        {
            get
            {
                return CategoryDescriptions.Keys.Any() ? CategoryDescriptions[CategoryDescriptions.Keys.First()] : Maybe.None;
            }
            set
            {
                if (!CategoryDescriptions.Keys.Any())
                {
                    CategoryDescriptions.Add("en-US", value);
                }
            }
        }

        public static CategoryItem UnfoundCategoryItem()
        {
            return new CategoryItem(0, Guid.Empty, "[Not Found]", 0);
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

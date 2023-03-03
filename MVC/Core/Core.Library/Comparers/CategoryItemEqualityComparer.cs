using Core.Models;

namespace Core.Comparers
{
    public class CategoryItemEqualityComparer : IEqualityComparer<CategoryItem>
    {
        public bool Equals(CategoryItem? x, CategoryItem? y)
        {
            return (x != null && y != null && x.ToObjectIdentity().Equals(y.ToObjectIdentity()) || (x == null && y == null));
        }

        public int GetHashCode(CategoryItem obj)
        {
            return obj.CategoryGuid.GetHashCode();
        }
    }
}

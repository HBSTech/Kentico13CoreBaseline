using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Generic.Models
{
    public class CategoryItemEqualityComparer : IEqualityComparer<CategoryItem>
    {
        public bool Equals(CategoryItem x, CategoryItem y)
        {
            return x.CategoryID == y.CategoryID;
        }

        public int GetHashCode([DisallowNull] CategoryItem obj)
        {
            return obj.CategoryGuid.GetHashCode();
        }
    }
}

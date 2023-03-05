using System;
using System.Collections.Generic;
using System.Linq;
namespace Core.Repositories
{
    /// <summary>
    /// These classes are specifically not-async, becuase often taxonomy operations are leveraged heavily,
    /// and the overhead of constantly needing async/await will be lost by the usual insignificant cached retrieval cost.
    /// </summary>
    public interface ICategoryCachedRepository
    {
        /// <summary>
        /// Gets the Category Identity (with all fields) given the category names
        /// </summary>
        /// <param name="categoryNames"></param>
        /// <returns></returns>
        IEnumerable<ObjectIdentity> CategoryNamesToCategoryIdentity(IEnumerable<string> categoryNames);

        /// <summary>
        /// Gets the CategoryItems based on the given Category Identity (id, code name, or guid).  You can use the string/int/guid.ToObjectIdentity() helper
        /// </summary>
        /// <param name="categoryNames"></param>
        /// <returns></returns>
        IEnumerable<CategoryItem> GetCategoryIdentifiertoCategoryCached(IEnumerable<ObjectIdentity> categoryIdentity);

        /// <summary>
        /// Gets the Category based on the given category object identity (id, code name, or guid).  You can use the string/int/guid.ToObjectIdentity() helper
        /// </summary>
        /// <param name="categoryName"></param>
        /// <returns></returns>
        Result<CategoryItem> GetCategoryIdentifiertoCategoryCached(ObjectIdentity categoryIdentity);

        /// <summary>
        /// Gets a dictionary of all the category Items by the ID
        /// </summary>
        /// <returns></returns>
        Dictionary<int, CategoryItem> GetCategoryCachedById();

        /// <summary>
        /// Gets a dictionary of all the category items by Code Name (each value is .ToLowerInvariant() for the keys)
        /// </summary>
        /// <returns></returns>
        Dictionary<string, CategoryItem> GetCategoryCachedByCodeName();

        /// <summary>
        /// Gets a dictionary of all hte category items by Guid
        /// </summary>
        /// <returns></returns>
        Dictionary<Guid, CategoryItem> GetCategoryCachedByGuid();
    }
}

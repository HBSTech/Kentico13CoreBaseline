using Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Localization.Models
{
    // Represented a version of the CategoryItem that has had it's Display Name and possible description localized
    public class LocalizedCategoryItem : CategoryItem
    {
        public LocalizedCategoryItem(int categoryID, Guid categoryGuid, string categoryName, int categoryParentID, string categoryDisplayName) : base(categoryID, categoryGuid, categoryName, categoryParentID, categoryDisplayName)
        {
        }
    }
}

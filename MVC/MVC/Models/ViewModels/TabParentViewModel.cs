using CMS.DocumentEngine.Types.Generic;
using System.Collections.Generic;

namespace Generic.ViewModels
{
    public class TabParentViewModel
    {
        public IEnumerable<Tab.TabFields> Tabs { get; internal set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SectionsSystem.Models.Sections
{
    public class WidgetSection
    {
        public WidgetSection(PageIdentity pageIdentity)
        {
            PageIdentity = pageIdentity;
        }

        public PageIdentity PageIdentity { get; }

        public bool FullWidth { get; set; } = false;
    }
}

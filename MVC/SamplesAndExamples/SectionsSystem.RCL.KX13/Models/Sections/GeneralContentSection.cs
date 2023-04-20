using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SectionsSystem.Models.Sections
{
    public class GeneralContentSection
    {
        public GeneralContentSection(TextAlignment generalContentTextAlignment)
        {
            TextAlignment = generalContentTextAlignment;
        }

        public TextAlignment TextAlignment { get; }
    }
}

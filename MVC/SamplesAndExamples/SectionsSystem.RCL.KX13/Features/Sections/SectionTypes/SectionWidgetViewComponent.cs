using SectionsSystem.Models.Sections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SectionsSystem.Features.Sections.SectionTypes
{
    [ViewComponent]
    public class SectionWidgetViewComponent : ViewComponent
    {

        public SectionWidgetViewComponent()
        {
        }

        public IViewComponentResult Invoke(Section<WidgetSection> xSection)
        {
            if (xSection.GetSectionPageIdentity().TryGetValue(out var page))
            {
                var model = new SectionWidgetViewModel(page.DocumentID, xSection.SectionModel.FullWidth);
                return View("/Features/Sections/SectionTypes/SectionWidget.cshtml", model);
            }
            return this.PageBuilderMessage("Missing Page Identity on Widget Section");
        }
    }

    public record SectionWidgetViewModel
    {
        public SectionWidgetViewModel(int documentID, bool fullWidth)
        {
            DocumentID = documentID;
            FullWidth = fullWidth;
        }

        public int DocumentID { get; set; }
        public bool FullWidth { get; set; }
    }
}

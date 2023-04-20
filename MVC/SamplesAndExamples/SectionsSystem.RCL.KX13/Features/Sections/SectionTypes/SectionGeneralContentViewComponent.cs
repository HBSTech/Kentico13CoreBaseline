using SectionsSystem.Models.SectionItems;
using SectionsSystem.Models.Sections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SectionsSystem.Features.Sections.SectionTypes
{
    [ViewComponent]
    public class SectionGeneralContentViewComponent : ViewComponent
    {
        private readonly ISectionItemRepository _sectionItemRepository;

        public SectionGeneralContentViewComponent(ISectionItemRepository sectionItemRepository)
        {
            _sectionItemRepository = sectionItemRepository;
        }

        public async Task<IViewComponentResult> InvokeAsync(Section<GeneralContentSection> xSection)
        {

            var sectionItem = await _sectionItemRepository.GetGeneralContentSectionItemAsync(xSection);
            if (sectionItem.TryGetValue(out var item))
            {
                var model = new SectionGeneralContentViewModel(xSection.SectionModel, item);
                return View("/Features/Sections/SectionTypes/SectionGeneralContent.cshtml", model);
            }

            return this.PageBuilderMessage(sectionItem.Error);

        }
    }

    public record SectionGeneralContentViewModel
    {
        public SectionGeneralContentViewModel(GeneralContentSection generalContent, BasicSectionItem item)
        {
            GeneralContent = generalContent;
            Item = item;
        }

        public GeneralContentSection GeneralContent { get; set; }
        public BasicSectionItem Item { get; set; }
    }
}

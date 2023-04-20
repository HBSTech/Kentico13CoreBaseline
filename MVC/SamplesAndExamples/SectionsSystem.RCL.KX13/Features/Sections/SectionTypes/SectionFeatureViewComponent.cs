using SectionsSystem.Models.SectionItems;
using SectionsSystem.Models.Sections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SectionsSystem.Features.Sections.SectionTypes
{
    public class SectionFeatureViewComponent : ViewComponent
    {
        private readonly ISectionItemRepository _sectionItemRepository;

        public SectionFeatureViewComponent(ISectionItemRepository sectionItemRepository)
        {
            _sectionItemRepository = sectionItemRepository;
        }

        public async Task<IViewComponentResult> InvokeAsync(Section<FeatureSection> xSection)
        {
            if (xSection.GetSectionPageIdentity().TryGetValue(out var pageIdentity))
            {
                var model = new SectionFeatureViewModel(xSection.SectionModel, await _sectionItemRepository.GetFeatureSectionItemsAsync(pageIdentity));
                return View("/Features/Sections/SectionTypes/SectionFeature.cshtml", model);

            }
            else
            {
                return this.PageBuilderMessage("No identity given to the Feature  Section, cannot retrieve items.");
            }
        }


    }

    public record SectionFeatureViewModel
    {
        public SectionFeatureViewModel(FeatureSection featureSection, IEnumerable<BasicSectionItem> features)
        {
            FeatureSection = featureSection;
            Features = features;
        }

        public FeatureSection FeatureSection { get; }
        public IEnumerable<BasicSectionItem> Features { get; }
    }
}

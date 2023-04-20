namespace SectionsSystem.Features.Sections
{
    [ViewComponent(Name = "RenderSection")]
    public class RenderSectionViewComponent : ViewComponent
    {
        private readonly ISectionRepository _sectionRepository;
        private readonly ICacheDependenciesScope _cacheDependenciesScope;

        public RenderSectionViewComponent(ISectionRepository sectionRepository,
            ICacheDependenciesScope cacheDependenciesScope)
        {
            _sectionRepository = sectionRepository;
            _cacheDependenciesScope = cacheDependenciesScope;
        }

        public async Task<IViewComponentResult> InvokeAsync(ISection? xSection = null, NodeIdentity? xSectionIdentity = null, SectionStyleType? xOverrideStyleType = null)
        {
            if (xSection != null)
            {
                var model = new RenderSectionViewModel(xSection)
                {
                    OverrideStyleType = xOverrideStyleType.AsMaybe()
                };
                return View("/Features/Sections/Section.cshtml", model);
            }
            else if (xSectionIdentity.AsMaybe().TryGetValue(out var sectionIdentity))
            {
                _cacheDependenciesScope.Begin();
                var foundSection = await _sectionRepository.GetSectionAsync(sectionIdentity);
                var dependencies = _cacheDependenciesScope.End();
                if (foundSection.TryGetValue(out var foundSectionVal))
                {
                    var model = new RenderSectionViewModel(foundSectionVal, dependencies)
                    {
                        OverrideStyleType = xOverrideStyleType.AsMaybe()
                    };
                    return View("/Features/Sections/Section.cshtml", model);
                }
            }
            return this.PageBuilderMessage("No Section Provided or no Section Found by that Node Identity", true, false);
        }
    }

    public record RenderSectionViewModel
    {
        public RenderSectionViewModel(ISection section)
        {
            Section = section;
            Cache = false;
        }

        public RenderSectionViewModel(ISection section, IEnumerable<string> dependencyKeys)
        {
            Section = section;
            DependencyKeys = dependencyKeys;
            Cache = true;
        }

        public ISection Section { get; set; }
        public Maybe<SectionStyleType> OverrideStyleType { get; set; }
        public IEnumerable<string> DependencyKeys { get; set; } = Array.Empty<string>();
        /// <summary>
        /// If this isn't called from the SectionsViewComponent (which has a entire section cache), then cache individually.
        /// </summary>
        public bool Cache { get; set; }
    }
}

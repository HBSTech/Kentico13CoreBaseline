namespace SectionsSystem.Features.Sections
{
    public class SectionsViewComponent : ViewComponent
    {
        private readonly ISectionRepository _sectionRepository;
        private readonly IPageContextRepository _pageContextRepository;
        private readonly ICacheDependenciesScope _cacheDependenciesScope;

        public SectionsViewComponent(ISectionRepository sectionRepository,
            IPageContextRepository pageContextRepository,
            ICacheDependenciesScope cacheDependenciesScope)
        {
            _sectionRepository = sectionRepository;
            _pageContextRepository = pageContextRepository;
            _cacheDependenciesScope = cacheDependenciesScope;
        }

        public async Task<IViewComponentResult> InvokeAsync(PageIdentity? xPageIdentity = null)
        {
            Maybe<PageIdentity> parentPageMaybe = xPageIdentity != null ? xPageIdentity : Maybe.None;
            if (!parentPageMaybe.HasValue)
            {
                parentPageMaybe = (await _pageContextRepository.GetCurrentPageAsync()).TryGetValue(out var page) ? page : Maybe.None;
            }

            if (parentPageMaybe.TryGetValue(out var parentPageVal))
            {
                _cacheDependenciesScope.Begin();

                var sections = await _sectionRepository.GetSectionsAsync(parentPageVal.NodeIdentity);
                var dependencies = _cacheDependenciesScope.End();
                if (sections.Any())
                {
                    var model = new SectionsViewModel(sections, parentPageVal, dependencies);
                    return View("/Features/Sections/Sections.cshtml", model);
                }

                // End scope since just returning and end page builder message
                return this.PageBuilderMessage("No Sections found, must either have a child Sections Folder or have Sections in the Sections Relationship (TAC -> Sections)", true, false);

            }

            return this.PageBuilderMessage("No page identity passed and could not retrieve the current page.  Please pass parent page identity manually.", true, true);
        }
    }
    public record SectionsViewModel
    {
        public SectionsViewModel(IEnumerable<ISection> sections, PageIdentity parentPage, IEnumerable<string> dependencyKeys)
        {
            Sections = sections;
            ParentPage = parentPage;
            DependencyKeys = dependencyKeys;
        }

        public IEnumerable<ISection> Sections { get; set; }
        public PageIdentity ParentPage { get; set; }
        public IEnumerable<string> DependencyKeys { get; set; }
    }
}

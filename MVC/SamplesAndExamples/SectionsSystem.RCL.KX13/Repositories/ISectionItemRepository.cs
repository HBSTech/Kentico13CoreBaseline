using SectionsSystem.Models.SectionItems;
using SectionsSystem.Models.Sections;

namespace SectionsSystem.Repositories
{
    public interface ISectionItemRepository
    {
        
        /// <summary>
        /// Get the General Content Section Items given teh parent section page
        /// </summary>
        /// <param name="parentSection"></param>
        /// <returns></returns>
		Task<Result<BasicSectionItem<GeneralContentSection>>> GetGeneralContentSectionItemAsync(Section<GeneralContentSection> parentSection);

        /// <summary>
        /// Get the Feature Grid Section Items given the parent section page
        /// </summary>
        /// <param name="parentPage"></param>
        /// <returns></returns>
		Task<IEnumerable<BasicSectionItem>> GetFeatureSectionItemsAsync(PageIdentity parentPage);


        /* Add more here for any sections where you need child items retrieved */

    }
}

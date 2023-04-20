using SectionsSystem.Enums;
using SectionsSystem.Interfaces;

namespace SectionsSystem.Models.Sections
{
    public class Section<T> : ISection
    {
        /// <summary>
        /// Creates a SectionItem model with additional model data.  The itemModel should correlate to the parent section's type and should be checked for the right type upon usage. 
        /// </summary>
        /// <param name="sectionModel"></param>
        public Section(T sectionModel)
        {
            SectionModel = sectionModel;
        }

        public T SectionModel { get; }

        public SectionStyleType StyleType { get; set; } = SectionStyleType.Default;

        public Maybe<ColorTheme> Theme { get; set; }

        public List<string> AdditionalCssItems { get; internal set; } = new List<string>();

        public Maybe<string> AdditionalCss => AdditionalCssItems.Any() ? string.Join(" ", AdditionalCssItems) : Maybe.None;
        public Maybe<PageIdentity> Page { get; set; }

        public Result<IImageSection> Image { get; set; } = Result.Failure<IImageSection>("Not set");
        public Result<IVideoSection> Video { get; set; } = Result.Failure<IVideoSection>("Not set");
        public bool Divider { get; set; } = false;

        public Maybe<string> GetAdditionalCss() => AdditionalCss;

        public Result<IImageSection> GetSectionImageSection() => Image;

        public Maybe<PageIdentity> GetSectionPageIdentity() => Page;

        public SectionStyleType GetSectionStyleType() => StyleType;

        public Result<IVideoSection> GetSectionVideoSection() => Video;

        public Maybe<ColorTheme> GetTheme() => Theme;

        public bool ShowDivider() => Divider;
    }
}

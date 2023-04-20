using SectionsSystem.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SectionsSystem.Interfaces
{
    public interface ISection
    {
        /// <summary>
        /// The Section page identity, used to find and render sections beneath
        /// </summary>
        /// <returns></returns>
        Maybe<PageIdentity> GetSectionPageIdentity();

        /// <summary>
        /// The styling of the section
        /// </summary>
        /// <returns></returns>
        SectionStyleType GetSectionStyleType();

        /// <summary>
        /// The theme color of the section
        /// </summary>
        /// <returns></returns>
        Maybe<ColorTheme> GetTheme();

        Maybe<string> GetAdditionalCss();

        /// <summary>
        /// The Video content of the section style, should only be called if GetSectionStyleType is Video
        /// </summary>
        /// <returns></returns>
        Result<IVideoSection> GetSectionVideoSection();

        /// <summary>
        /// The Image content of the section style, Should only be called if GetSectionStyleType is Image or ParallaxImage
        /// </summary>
        /// <returns></returns>
        Result<IImageSection> GetSectionImageSection();

        /// <summary>
        /// If true, show a divider after
        /// </summary>
        /// <returns></returns>
        bool ShowDivider();
    }
}

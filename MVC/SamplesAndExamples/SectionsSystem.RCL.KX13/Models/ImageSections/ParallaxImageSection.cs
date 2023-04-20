using SectionsSystem.Interfaces;

namespace SectionsSystem.Models.ImageSections
{
    public class ParallaxImageSection : IImageSection
    {
        public ParallaxImageSection(string ImageUrl)
        {
            this.ImageUrl = ImageUrl;
        }

        public string ImageUrl { get; }

        public string GetImageUrl() => ImageUrl;
    }
}

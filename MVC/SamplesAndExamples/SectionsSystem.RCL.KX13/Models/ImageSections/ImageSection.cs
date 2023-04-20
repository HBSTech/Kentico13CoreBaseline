using SectionsSystem.Interfaces;

namespace SectionsSystem.Models.ImageSections
{
    public class ImageSection : IImageSection
    {
        public ImageSection(string ImageUrl)
        {
            this.ImageUrl = ImageUrl;
        }

        public string ImageUrl { get; }

        public string GetImageUrl() => ImageUrl;
    }
}

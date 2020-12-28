using Generic.InlineEditors;
using System;

namespace Generic.ViewModels
{
    /// <summary>
    /// View model for Image widget.
    /// </summary>
    public class ImageWidgetViewModel
    {
        public Guid ImageGuid { get; set; }
        /// <summary>
        /// Image.
        /// </summary>
        public string ImageUrl { get; set; }

        public string Alt { get; set; }

        public string CssClass { get; set; }
        public ImageTypeEnum ImageType { get; set; }
    }
}
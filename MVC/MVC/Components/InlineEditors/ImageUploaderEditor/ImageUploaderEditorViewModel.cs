using Models.InlineEditors;

namespace Generic.InlineEditors
{
    /// <summary>
    /// View model for Image uploader editor.
    /// </summary>
    public class ImageUploaderEditorViewModel : InlineEditorViewModel
    {
        /// <summary>
        /// Indicates if image is present.
        /// </summary>
        public bool HasImage { get; set; }


        /// <summary>
        /// Indicates if the message should be positioned absolutely for empty image.
        /// </summary>
        public bool UseAbsolutePosition { get; set; }


        /// <summary>
        /// Position of the message in case of absolute position.
        /// </summary>
        public PanelPositionEnum MessagePosition { get; set; } = PanelPositionEnum.Center;


        /// <summary>
        /// Type of the uploaded image.
        /// </summary>
        public ImageTypeEnum ImageType { get; set; } = ImageTypeEnum.Attachment;


        /// <summary>
        /// Url to image uploader API.
        /// </summary>
        public string DataUrl { get; set; }
    }
}
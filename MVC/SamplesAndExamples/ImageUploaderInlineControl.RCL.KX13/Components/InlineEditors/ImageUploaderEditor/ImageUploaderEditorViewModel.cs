
using ImageUploaderInlineControl.Models;

namespace ImageUploaderInlineControl.Components.InlineEditors.ImageUploaderEditor
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
        public UploaderImageTypeEnum ImageType { get; set; } = UploaderImageTypeEnum.Attachment;


        /// <summary>
        /// Url to image uploader API.
        /// </summary>
        public string DataUrl { get; set; }

        /// <summary>
        /// What media library to upload into
        /// </summary>
        public string MediaLibrary { get; set; }
    }
}
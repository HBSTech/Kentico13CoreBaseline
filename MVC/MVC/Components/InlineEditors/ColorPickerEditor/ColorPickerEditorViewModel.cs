using Models.InlineEditors;

namespace Generic.InlineEditors
{
    /// <summary>
    /// View model for Color picker editor.
    /// </summary>
    public sealed class ColorPickerEditorViewModel : InlineEditorViewModel
    {
        /// <summary>
        /// Color CSS class.
        /// </summary>
        public string ColorCssClass { get; set; }
    }
}
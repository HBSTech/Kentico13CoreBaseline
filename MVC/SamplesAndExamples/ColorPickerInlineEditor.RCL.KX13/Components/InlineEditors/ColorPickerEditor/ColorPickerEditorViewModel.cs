using Core.KX13.Models.InlineEditors;

namespace ColorPickerInlineEditor.Components.InlineEditors.ColorPickerEditor
{
    /// <summary>
    /// View model for Color picker editor.
    /// </summary>
    public sealed class ColorPickerEditorViewModel : InlineEditorViewModel
    {
        public ColorPickerEditorViewModel(string propertyName, string colorCssClass) : base(propertyName)
        {
            ColorCssClass = colorCssClass;
        }

        /// <summary>
        /// Color CSS class.
        /// </summary>
        public string ColorCssClass { get; set; }
    }
}
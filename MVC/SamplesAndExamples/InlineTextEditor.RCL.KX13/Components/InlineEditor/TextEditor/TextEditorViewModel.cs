using Core.KX13.Models.InlineEditors;

namespace InlineTextEditor.Components.InlineEditors.TextEditor
{
    /// <summary>
    /// View model for Text editor.
    /// </summary>
    public sealed class TextEditorViewModel : InlineEditorViewModel
    {
        public TextEditorViewModel(string propertyName, string text) : base(propertyName)
        {
            Text = text;
        }

        /// <summary>
        /// Editor text.
        /// </summary>
        public string Text { get; set; }


        /// <summary>
        /// Placeholder text.
        /// </summary>
        public string PlaceholderText { get; set; } = "Type your text";
    }
}
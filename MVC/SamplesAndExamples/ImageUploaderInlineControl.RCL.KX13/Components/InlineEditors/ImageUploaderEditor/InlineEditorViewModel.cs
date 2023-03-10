namespace ImageUploaderInlineControl.Components.InlineEditors.ImageUploaderEditor
{
    /// <summary>
    /// Base class for inline editor view models.
    /// </summary>
    public abstract class InlineEditorViewModel
    {
        /// <summary>
        /// Name of the widget property to edit.
        /// </summary>
        public string PropertyName { get; set; }

        protected InlineEditorViewModel(string propertyName)
        {
            PropertyName = propertyName;
        }
    }
}
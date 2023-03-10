namespace Core.KX13.Models.InlineEditors;

/// <summary>
/// Base class for inline editor view models.
/// </summary>
public abstract class InlineEditorViewModel
{
    protected InlineEditorViewModel(string propertyName)
    {
        PropertyName = propertyName;
    }

    /// <summary>
    /// Name of the widget property to edit.
    /// </summary>
    public string PropertyName { get; set; }
}
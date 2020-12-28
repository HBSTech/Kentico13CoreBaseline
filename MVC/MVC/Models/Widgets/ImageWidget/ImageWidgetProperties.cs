using System;
using System.Collections.Generic;
using Kentico.Components.Web.Mvc.FormComponents;
using Kentico.Forms.Web.Mvc;
using Kentico.PageBuilder.Web.Mvc;

namespace Generic.Models
{
    /// <summary>
    /// Properties of Image widget.
    /// </summary>
    public class ImageWidgetProperties : IWidgetProperties
    {
        /// <summary>
        /// Guid of an image to be displayed.
        /// </summary>
        public string ImageGuid { get; set; }

        [EditingComponent(CheckBoxComponent.IDENTIFIER, Label ="Use Attachment", Tooltip ="Uncheck if you wish to use the below media library path.", Order = 1)]
        public bool UseAttachment { get; set; } = true;

        [EditingComponent(MediaFilesSelector.IDENTIFIER, Label = "Media relative link", Order = 2)]
        [EditingComponentProperty(nameof(MediaFilesSelectorProperties.MaxFilesLimit), 1)]
        [EditingComponentProperty(nameof(MediaFilesSelectorProperties.AllowedExtensions), ".gif;.png;.jpg;.jpeg")]
        [VisibilityCondition(nameof(UseAttachment), ComparisonTypeEnum.IsFalse)]
        public IEnumerable<MediaFilesSelectorItem> ImageUrl { get; set; }

        [EditingComponent(TextInputComponent.IDENTIFIER, Label = "Image Alt", Order = 3)]
        public string Alt { get; set; }

        [EditingComponent(TextInputComponent.IDENTIFIER, Label = "CSS Class", Order = 4)]
        public string CssClass { get; set; }
    }
}
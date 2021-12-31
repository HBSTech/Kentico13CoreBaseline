﻿using Kentico.PageBuilder.Web.Mvc.PageTemplates;
using Generic.Features.ShareableContent;

[assembly: RegisterPageTemplate(
    "Generic.ShareableContent_Default",
    "ShareableContent",
    typeof(ShareableContentPageTemplateProperties),
    "~/Features/ShareableContent/ShareableContentPageTemplate.cshtml")]

namespace Generic.Features.ShareableContent
{
    public class ShareableContentPageTemplateFilter : PageTypePageTemplateFilter
    {
        public override string PageTypeClassName => "Generic.ShareableContent";
    }

    public class ShareableContentPageTemplateProperties : IPageTemplateProperties
    {
        // Custom Properties here
    }
}
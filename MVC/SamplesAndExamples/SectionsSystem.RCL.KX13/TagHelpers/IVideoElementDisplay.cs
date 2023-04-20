using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;
using SectionsSystem.Models.VideoSections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace SectionsSystem.TagHelpers
{
    [HtmlTargetElement("bs-video-element-display")]

    public class StructureVideoTagHelper : TagHelper
    {

        public IVideoSection? xVideoSection { get; set; }
        public bool xVideoAutoplay { get; set; } = false;
        public bool xVideoMuted { get; set; } = false;
        public bool xVideoLoop { get; set; } = false;
        /*
         * Youtube values
         * , t = e.attr("data-video")
          , i = e.attr("data-mute") || !0
          , n = e.attr("data-ratio") || "16/9"
          , s = e.attr("data-quality") || "hd720"
          , a = e.attr("data-opacity") || 1
          , r = e.attr("data-container") || "parent"
          , o = e.attr("data-optimize") || !0
          , l = e.attr("data-loop") || !0
          , c = e.attr("data-controls") || !1
          , d = e.attr("data-volume") || 50
          , u = e.attr("data-start") || 0
          , p = e.attr("data-stop") || 0
          , h = e.attr("data-autoplay") || !0
          , f = e.attr("data-fullscreen") || !1;*/

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (xVideoSection != null)
            {
                output.TagName = "div";
                output.AddClass("video-wrap", HtmlEncoder.Default);

                if (xVideoSection is Html5VideoSection html5VideoSection)
                {
                    var html5Div = new TagBuilder("video");
                    html5Div.Attributes.Add("preload", "auto");

                    if (html5VideoSection.ThumbnailUrl.TryGetValue(out var thumb))
                    {
                        html5Div.Attributes.Add("poster", thumb);
                    }
                    if (xVideoAutoplay)
                    {
                        html5Div.Attributes.Add("autoplay", "autoplay");
                    }
                    if (xVideoMuted)
                    {
                        html5Div.Attributes.Add("muted", "muted");
                    }
                    if (xVideoLoop)
                    {
                        html5Div.Attributes.Add("loop", "loop");
                    }
                    if (html5VideoSection.Mp4Url.TryGetValue(out var mp4))
                    {
                        html5Div.InnerHtml.AppendHtml($"<source src=\"{mp4}\" type=\"video/mp4\" />");
                    }
                    if (html5VideoSection.OggUrl.TryGetValue(out var ogg))
                    {
                        html5Div.InnerHtml.AppendHtml($"<source src=\"{ogg}\" type=\"video/ogg\" />");
                    }
                    if (html5VideoSection.WebMUrl.TryGetValue(out var webm))
                    {
                        html5Div.InnerHtml.AppendHtml($"<source src=\"{webm}\" type=\"video/webm\" />");
                    }
                    output.Content.AppendHtml(html5Div);
                    output.Content.AppendHtml("<div class=\"video-overlay\"></div>");

                }
                else if (xVideoSection is YoutubeVideoSection youtubeSection)
                {
                    var youtubeDiv = new TagBuilder("div");
                    youtubeDiv.AddCssClass("yt-bg-player");
                    youtubeDiv.Attributes.Add("data-quality", "hd1080");
                    youtubeDiv.Attributes.Add("data-video", youtubeSection.YoutubeVideoID);
                    if (xVideoAutoplay)
                    {
                        youtubeDiv.Attributes.Add("data-autoplay", "true");
                    }
                    if (xVideoMuted)
                    {
                        youtubeDiv.Attributes.Add("data-mute", "true");
                    }
                    if (xVideoLoop)
                    {
                        youtubeDiv.Attributes.Add("data-loop", "true");
                    }
                    output.Content.SetHtmlContent(youtubeDiv);
                }
            }
        }
    }
}

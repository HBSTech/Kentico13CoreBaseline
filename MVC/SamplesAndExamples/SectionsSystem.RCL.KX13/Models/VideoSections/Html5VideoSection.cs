using SectionsSystem.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SectionsSystem.Models.VideoSections
{
    /// <summary>
    /// Video Section for Html5 SectionStyleType
    /// </summary>
    public class Html5VideoSection : IVideoSection
    {
        public Html5VideoSection(Maybe<string> mp4Url, Maybe<string> webMUrl, Maybe<string> oggUrl)
        {
            Mp4Url = mp4Url;
            WebMUrl = webMUrl;
            OggUrl = oggUrl;
            ThumbnailUrl = Maybe.None;
        }

        public Html5VideoSection(Maybe<string> mp4Url, Maybe<string> webMUrl, Maybe<string> oggUrl, string thumbnailUrl)
        {
            Mp4Url = mp4Url;
            WebMUrl = webMUrl;
            OggUrl = oggUrl;
            ThumbnailUrl = thumbnailUrl;
        }

        public Maybe<string> Mp4Url { get; }
        public Maybe<string> WebMUrl { get; }
        public Maybe<string> OggUrl { get; }
        public Maybe<string> ThumbnailUrl { get; }

        public SectionVideoSourceType GetVideoSourceType() => SectionVideoSourceType.Html5Video;
    }
}

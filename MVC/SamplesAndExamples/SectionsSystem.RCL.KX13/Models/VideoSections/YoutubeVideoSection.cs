using SectionsSystem.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace SectionsSystem.Models.VideoSections
{
    /// <summary>
    /// Video Section for YouTube SectionStyleType
    /// </summary>
    public class YoutubeVideoSection : IVideoSection
    {
        /// <summary>
        /// Constructor, tries handles if the youtube VideoID is a URL instead of just hte media id.
        /// </summary>
        /// <param name="youtubeVideoID"></param>
        public YoutubeVideoSection(string youtubeVideoID)
        {
            if (youtubeVideoID.Contains("http"))
            {
                YoutubeVideoID = GetYouTubeVideoIdFromUrl(youtubeVideoID);
            }
            else
            {
                YoutubeVideoID = youtubeVideoID;
            }
        }

        public string YoutubeVideoID { get; }

        public SectionVideoSourceType GetVideoSourceType() => SectionVideoSourceType.Youtube;

        static private string GetYouTubeVideoIdFromUrl(string url)
        {
            Uri? uri;
            if (!Uri.TryCreate(url, UriKind.Absolute, out uri))
            {
                try
                {
                    uri = new UriBuilder("http", url).Uri;
                }
                catch
                {
                    // invalid url
                    return "";
                }
            }

            string host = uri.Host;
            string[] youTubeHosts = { "www.youtube.com", "youtube.com", "youtu.be", "www.youtu.be" };
            if (!youTubeHosts.Contains(host))
                return "";

            var query = HttpUtility.ParseQueryString(uri.Query);

            if (query.AllKeys.Contains("v") && query["v"].AsMaybe().TryGetValue(out var v))
            {
                return Regex.Match(v, @"^[a-zA-Z0-9_-]{11}$").Value;
            }
            else if (query.AllKeys.Contains("u") && query["u"].AsMaybe().TryGetValue(out var u))
            {
                // some urls have something like "u=/watch?v=AAAAAAAAA16"
                return Regex.Match(u, @"/watch\?v=([a-zA-Z0-9_-]{11})").Groups[1].Value;
            }
            else
            {
                // remove a trailing forward space
                var last = uri.Segments.Last().Replace("/", "");
                if (Regex.IsMatch(last, @"^v=[a-zA-Z0-9_-]{11}$"))
                    return last.Replace("v=", "");

                string[] segments = uri.Segments;
                if (segments.Length > 2 && segments[segments.Length - 2] != "v/" && segments[segments.Length - 2] != "watch/")
                    return "";

                return Regex.Match(last, @"^[a-zA-Z0-9_-]{11}$").Value;
            }
        }
    }
}

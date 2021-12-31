using System;

namespace Generic.Models
{
    public class MediaItem
    {
        public Guid MediaGUID { get; set; }
        public string MediaName { get; set; }
        public string MediaTitle { get; set; }
        public string MediaDescription { get; set; }
        public string MediaExtension { get; set; }
        public string MediaUrl { get; set; }
        public string MediaPermanentUrl { get; set; }

    }
}

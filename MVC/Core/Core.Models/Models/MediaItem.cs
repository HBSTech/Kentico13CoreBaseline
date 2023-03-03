namespace Core.Models
{
    public class MediaItem
    {
        public MediaItem(Guid mediaGUID, string mediaName, string mediaTitle, string mediaExtension, string mediaUrl, string mediaPermanentUrl)
        {
            MediaGUID = mediaGUID;
            MediaName = mediaName;
            MediaTitle = mediaTitle;
            MediaExtension = mediaExtension;
            MediaUrl = mediaUrl;
            MediaPermanentUrl = mediaPermanentUrl;
        }

        public Guid MediaGUID { get; set; }
        public string MediaName { get; set; }
        public string MediaTitle { get; set; }
        public Maybe<string> MediaDescription { get; set; }
        public string MediaExtension { get; set; }
        public string MediaUrl { get; set; }
        public string MediaPermanentUrl { get; set; }

    }
}

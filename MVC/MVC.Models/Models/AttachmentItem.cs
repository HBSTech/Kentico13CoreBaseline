using System;

namespace Generic.Models
{
    public class AttachmentItem
    {
        public Guid AttachmentGUID { get; set; }
        public string AttachmentName { get; set; }
        public string AttachmentTitle { get; set; }
        public string AttachmentExtension { get; set; }
        public string AttachmentUrl { get; set; }
    }
}

using System;

namespace Generic.Models
{
    public class RoleItem
    {
        public int SiteID { get; set; }
        public int RoleID { get; set; }
        public string RoleDisplayName { get; set; }
        public string RoleName { get; set; }
        public string RoleDescription { get; set; }
        public Guid RoleGUID { get; set; }
        public bool RoleIsDomain { get; set; }
    }
}

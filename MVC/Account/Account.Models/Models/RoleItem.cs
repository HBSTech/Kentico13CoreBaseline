namespace Account.Models
{
    public class RoleItem : IObjectIdentifiable
    {
        public RoleItem(ObjectIdentity siteID, int roleID, string roleDisplayName, string roleName, Guid roleGUID)
        {
            Site = siteID;
            RoleID = roleID;
            RoleDisplayName = roleDisplayName;
            RoleName = roleName;
            RoleGUID = roleGUID;
        }

        public ObjectIdentity Site { get; set; }
        public int RoleID { get; set; }
        public string RoleDisplayName { get; set; }
        public string RoleName { get; set; }
        public Maybe<string> RoleDescription { get; set; }
        public Guid RoleGUID { get; set; }
        public bool RoleIsDomain { get; set; } = false;

        public ObjectIdentity ToObjectIdentity()
        {
            return new ObjectIdentity()
            {
                CodeName = RoleName,
                Guid = RoleGUID,
                Id = RoleID
            };
        }
    }
}

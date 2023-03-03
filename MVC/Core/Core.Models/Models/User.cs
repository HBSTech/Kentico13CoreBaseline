namespace Core.Models
{
    public class User : IObjectIdentifiable
    {
        public User(string userName, string firstName, string lastName, string email, bool enabled, bool isExternal, bool isPublic)
        {
            UserName = userName;
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            Enabled = enabled;
            IsExternal = isExternal;
            IsPublic = isPublic;
        }

        public User(int userID, string userName, Guid userGUID, string email, string firstName, string middleName, string lastName, bool enabled, bool isExternal, bool isPublic = false)
        {
            UserID = userID;
            UserName = userName;
            UserGUID = userGUID;
            Email = email;
            FirstName = firstName;
            LastName = lastName;
            Enabled = enabled;
            IsExternal = isExternal;
            IsPublic = isPublic;
        }


        public Maybe<int> UserID { get; set; }
        public string UserName { get; set; }
        public Maybe<Guid> UserGUID { get; set; }

        public string Email { get; set; }

        public string FirstName { get; set; }
        public Maybe<string> MiddleName { get; set; }
        public string LastName { get; set; }
        public bool Enabled { get; set; }
        public bool IsExternal { get; set; }
        public bool IsPublic { get; set; }

        public ObjectIdentity ToObjectIdentity()
        {
            return new ObjectIdentity()
            {
                Id = UserID,
                CodeName = UserName,
                Guid = UserGUID
            };
        }
    }
}
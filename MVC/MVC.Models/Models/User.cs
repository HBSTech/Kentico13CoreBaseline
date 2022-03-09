using System;

namespace Generic.Models
{
    public class User
    {
        public int UserID { get; set; }
        public string UserName { get; set; }
        public Guid UserGUID { get; set; }

        public string Email { get; set; }

        public string FirstName { get; set; }
        public string MiddleNamge { get; set; }
        public string LastName { get; set; }
        public bool Enabled { get; set; }
        public bool IsExternal { get; set; }
    }
}

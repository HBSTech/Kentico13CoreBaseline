using Core.Models;

namespace Core
{
    public static class ToObjectIdentityHelper
    {
        public static ObjectIdentity ToObjectIdentity(this int value)
        {
            return new ObjectIdentity()
            {
                Id = value
            };
        }
        public static ObjectIdentity ToObjectIdentity(this string value)
        {
            return new ObjectIdentity()
            {
                CodeName = value
            };
        }
        public static ObjectIdentity ToObjectIdentity(this Guid value)
        {
            return new ObjectIdentity()
            {
                Guid = value
            };
        }
    }
}

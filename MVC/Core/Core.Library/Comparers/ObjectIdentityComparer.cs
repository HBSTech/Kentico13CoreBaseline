using Core.Models;
using System.Diagnostics.CodeAnalysis;

namespace Core.Comparers
{
    public class ObjectIdentityEqualityComparer : IEqualityComparer<ObjectIdentity>
    {
        public bool Equals(ObjectIdentity? x, ObjectIdentity? y)
        {
            if(x == null && y == null)
            {
                return true;
            }
            if (x == null || y == null)
            {
                return false;
            }
            if(
                (x.Id.TryGetValue(out var idValX) && y.Id.TryGetValue(out var idValY) && idValX == idValY)
                ||
                (x.CodeName.TryGetValue(out var codeValX) && y.CodeName.TryGetValue(out var codeValY) && codeValX.Equals(codeValY, StringComparison.OrdinalIgnoreCase))
                ||
                (x.Guid.TryGetValue(out var guidValX) && y.Guid.TryGetValue(out var guidValY) && guidValX == guidValY)){
                return true;
            }
            return false;

        }

        public int GetHashCode([DisallowNull] ObjectIdentity obj)
        {
            return $"{obj.Id.GetValueOrDefault(0)}{obj.CodeName.GetValueOrDefault(string.Empty)}{obj.Guid.GetValueOrDefault(Guid.Empty)}".GetHashCode();
        }
    }
}

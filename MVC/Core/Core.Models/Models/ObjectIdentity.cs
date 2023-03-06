using MVCCaching;

namespace Core.Models
{
    public record ObjectIdentity : ICacheKey
    {
        public Maybe<int> Id { get; set; }
        public Maybe<string> CodeName { get; set; }
        public Maybe<Guid> Guid { get; set; }

        public string GetCacheKey()
        {
            return $"{Id.GetValueOrDefault(0)}{CodeName.GetValueOrDefault(string.Empty)}{Guid.GetValueOrDefault(System.Guid.Empty)}";

        }

        public override int GetHashCode()
        {
            return GetCacheKey().GetHashCode();
        }
    }
}

namespace Core.Models
{
    public record ObjectIdentity
    {
        public Maybe<int> Id { get; set; }
        public Maybe<string> CodeName { get; set; }
        public Maybe<Guid> Guid { get; set; }

        public override int GetHashCode()
        {
            return $"{Id.GetValueOrDefault(0)}{CodeName.GetValueOrDefault(string.Empty)}{Guid.GetValueOrDefault(System.Guid.Empty)}".GetHashCode();
        }
    }
}

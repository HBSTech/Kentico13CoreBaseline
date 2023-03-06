namespace Core.Services
{
    public interface IIdentityService
    {
        Task<Result<NodeIdentity>> HydrateNodeIdentity(NodeIdentity identity);
        Task<Result<DocumentIdentity>> HydrateDocumentIdentity(DocumentIdentity identity);
        Task<Result<ObjectIdentity>> HydrateObjectIdentity(ObjectIdentity identity, string className);
    }
}

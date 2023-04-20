using Core.Enums.Structure;

namespace Core.Interfaces
{
    public interface IMediaItem
    {
        MediaType GetMediaType();

        Maybe<GeneralLink> GetLink();
    }
}

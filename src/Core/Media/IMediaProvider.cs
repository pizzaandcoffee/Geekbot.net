namespace Geekbot.Core.Media
{
    public interface IMediaProvider
    {
        string GetMedia(MediaType type);
    }
}
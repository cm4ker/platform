namespace ZenPlatform.ServerClientShared.Network
{
    public interface IRouteFactory
    {
        Route Create(string path);
    }
}
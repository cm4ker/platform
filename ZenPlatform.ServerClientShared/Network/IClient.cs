using System.IO;
using System.Net;
using ZenPlatform.Core.Authentication;

namespace ZenPlatform.Core.Network
{
    public interface IClient
    {
        bool Authenticated { get; }
        bool Connected { get; }
        string Database { get; }
        ConnectionInfo Info { get; }
        bool IsUse { get; }

        bool Authentication(IAuthenticationToken token);
        void Close();
        void Connect(IPEndPoint endPoint);
        T GetService<T>();
        TResponce Invoke<TResponce>(Route route, params object[] args);
        Stream InvokeStream(Route route, params object[] args);
        bool Use(string name);
    }
}
using System.IO;
using ZenPlatform.Core.Network;

namespace ZenPlatform.Core.Contracts.Network
{
    public delegate object ParametricMethod(InvokeContext context, params object[] list);
    public delegate void StreamMethod(InvokeContext context, Stream stream, params object[] list);
}
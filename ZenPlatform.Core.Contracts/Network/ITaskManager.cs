using System;
using System.Threading;
using System.Threading.Tasks;
using ZenPlatform.Core.Sessions;

namespace ZenPlatform.Core.Network
{

    public interface ITaskManager
    {
        Task<object> RunTask(ISession session, Func<InvokeContext, object> action);

    }


}
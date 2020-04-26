using System;
using System.Threading;
using System.Threading.Tasks;
using ZenPlatform.Core.Contracts;

namespace ZenPlatform.Core.Network
{

    public interface ITaskManager
    {
        Task<object> RunTask(ISession session, Func<InvokeContext, object> action);

    }


}
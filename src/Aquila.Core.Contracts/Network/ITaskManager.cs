using System;
using System.Threading;
using System.Threading.Tasks;
using Aquila.Core.Contracts;

namespace Aquila.Core.Network
{

    public interface ITaskManager
    {
        Task<object> RunTask(ISession session, Func<InvokeContext, object> action);

    }


}
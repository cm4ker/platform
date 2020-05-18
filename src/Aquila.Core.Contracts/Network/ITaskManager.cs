using System;
using System.Threading.Tasks;

namespace Aquila.Core.Contracts.Network
{

    public interface ITaskManager
    {
        Task<object> RunTask(ISession session, Func<InvokeContext, object> action);

    }


}
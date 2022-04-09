using System;
using System.Threading.Tasks;

namespace Aquila.Core.Contracts.Network
{

    public interface ITaskManager
    {
        Task<object> RunTask(Func<InvokeContext, object> action);

    }


}
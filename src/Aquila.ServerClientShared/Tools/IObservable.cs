using System;
using System.Collections.Generic;
using System.Text;

namespace Aquila.Core.Tools
{
    public interface IObservable<TMessage, TSender>
    {
        IDisposable Subscribe(IObserver<TMessage, TSender> observer);
    }
}

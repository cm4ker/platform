using System;
using System.Collections.Generic;
using System.Text;

namespace ZenPlatform.Core.Tools
{
    public interface IObserver<TValue, TSender>
    {
        void OnCompleted();
        void OnError(Exception error);
        void OnNext(TSender sender, TValue value);
    }
}

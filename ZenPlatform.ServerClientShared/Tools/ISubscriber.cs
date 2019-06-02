using System;
using System.Collections.Generic;
using System.Text;

namespace ZenPlatform.ServerClientShared.Tools
{
    public interface ISubscriber: IDisposable
    {
        void SetUnsubscriber(IDisposable unsubscriber);
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace ZenPlatform.ServerClientShared.Tools
{
    public class ListUnsubscriber<T> : IDisposable
    {
        private readonly List<T> subscribers;
        private readonly T subscriber;

        public ListUnsubscriber(List<T> subscribers, T subscriber)
        {
            this.subscribers = subscribers;
            this.subscriber = subscriber;
        }

        public void Dispose()
        {
            subscribers.Remove(subscriber);
        }
    }
}

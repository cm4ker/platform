using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading;
using ZenPlatform.Core.Tools;

namespace ZenPlatform.Core.Network
{
    public class NetworkProxyFactory
    {
        public T Create<T>(IConnection connection)
        {
            object proxy = DispatchProxy.Create<T, NetworkDispatchProxy>();



            ((NetworkDispatchProxy)proxy).Initialization(connection, typeof(T).AssemblyQualifiedName);


            return (T)proxy;
        }
    }
    public class NetworkDispatchProxy : DispatchProxy, IConnectionObserver<IConnectionContext> ,IDisposable
    {
        private IConnection _connection;
        private Guid _Id;
        private ConcurrentDictionary<Guid, Action<INetworkMessage>> _resultCallbacks;

        public NetworkDispatchProxy()
        {
            _resultCallbacks = new ConcurrentDictionary<Guid, Action<INetworkMessage>>();
        }

        public void Initialization(IConnection connection, string name)
        {
            _connection = connection;

            _connection.Subscribe(this);
            var requets = new RequestInvokeInstanceProxy(name);

            _Id = requets.Id;

            _connection.Channel.Send(requets);
        }

        protected override object Invoke(MethodInfo targetMethod, object[] args)
        {
            var request = new RequestInvokeMethodProxy(_Id, targetMethod.Name, args);
            object result = null;
            var wait = RequestAsync(request, m =>
            {
                if (m is ResponceInvokeMethodProxy responce)
                {
                    result = responce.Result;
                }
            });

            wait.WaitOne();

            return result;
        }

        private WaitHandle RequestAsync(INetworkMessage message, Action<INetworkMessage> CallBack)
        {
            AutoResetEvent restEvent = new AutoResetEvent(false);
            _resultCallbacks.TryAdd(message.Id, (m) => { CallBack(m); _resultCallbacks.TryRemove(message.Id, out _); restEvent.Set(); });
            _connection.Channel.Send(message);

            return restEvent;


        }

        #region IDisposable Support
        private bool disposedValue = false; // Для определения избыточных вызовов

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: освободить управляемое состояние (управляемые объекты).
                }

                // TODO: освободить неуправляемые ресурсы (неуправляемые объекты) и переопределить ниже метод завершения.
                // TODO: задать большим полям значение NULL.

                _connection.Channel.Send(new RequestInvokeDisposeProxy(_Id));

                disposedValue = true;
            }
        }

        // TODO: переопределить метод завершения, только если Dispose(bool disposing) выше включает код для освобождения неуправляемых ресурсов.
         ~NetworkDispatchProxy()
         {
          // Не изменяйте этот код. Разместите код очистки выше, в методе Dispose(bool disposing).
            Dispose(false);
         }

        // Этот код добавлен для правильной реализации шаблона высвобождаемого класса.
        public void Dispose()
        {
            // Не изменяйте этот код. Разместите код очистки выше, в методе Dispose(bool disposing).
            Dispose(true);
            // TODO: раскомментировать следующую строку, если метод завершения переопределен выше.
             GC.SuppressFinalize(this);
        }
        #endregion
     
        public void OnCompleted(IConnectionContext sender)
        {
           // throw new NotImplementedException();
        }

        public void OnError(IConnectionContext sender, Exception error)
        {
          //  throw new NotImplementedException();
        }

        public void OnNext(IConnectionContext context, INetworkMessage value)
        {
            if (_resultCallbacks.ContainsKey(value.RequestId))
            {
                _resultCallbacks[value.RequestId](value);
            }
        }

        public bool CanObserve(Type type)
        {
            return type.Equals(typeof(ResponceInvokeMethodProxy));
        }
    }
}

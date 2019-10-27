using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using ZenPlatform.Core.Tools;
using ZenPlatform.Core.Logging;
using Newtonsoft.Json;

namespace ZenPlatform.Core.Network
{
    public class Channel : IChannel
    {
        private byte[] _readBuffer;
        private int _bufferSize = 1024 * 4; // 4Kb
        private Stream _stream;
        private readonly IMessagePackager _packager;
        private readonly ILogger _logger;
        private readonly List<IObserver<INetworkMessage>> _observers;

        public bool Running { get; private set; } = false;


        //public event Action<Exception> OnError;

        public Channel(IMessagePackager packager, ILogger<Channel> logger)
        {
            _logger = logger;
            _readBuffer = new byte[_bufferSize];
            _packager = packager;
            _observers = new List<IObserver<INetworkMessage>>();
        }

        private void ReceiveCallback(IAsyncResult ar)
        {
            if (!Running) return;

            try
            {
                var bytesRead = _stream.EndRead(ar);
                if (bytesRead > 0)
                {
                    var messages = _packager.UnpackMessages(_readBuffer.AsSpan(0, bytesRead).ToArray());

                    foreach (var message in messages)
                    {
                        _logger.Trace(() => string.Format("From: ''; Type: '{0}'; Message: {1}",
                            message.GetType().Name, JsonConvert.SerializeObject(message)));

                        if (message is INetworkMessage networkMessage)
                            OnNext(networkMessage);
                        else _logger.Warn("Received message is unknown type: {0}", message.GetType().Name);
                    }
                }
                else
                {
                    Stop();
                }

                if (Running)
                {
                    _stream.BeginRead(_readBuffer, 0, _readBuffer.Length, new AsyncCallback(ReceiveCallback), null);
                }
            }
            catch (Exception ex)
            {
                OnError(ex);
                Stop();
            }
        }

        // todo send async method
        public void Send(object message)
        {
            if (!Running) throw new InvalidOperationException("Channel must be running.");
            if (message == null) throw new ArgumentNullException(nameof(message));
            _logger.Trace(() => string.Format("To: ''; Type: '{0}'; Message: {1}",
                message.GetType().Name, JsonConvert.SerializeObject(message)));

            var bytes = _packager.PackMessage(message);

            _stream.Write(bytes, 0, bytes.Length);
        }

        public void Start(Stream stream)
        {
            if (stream == null) throw new ArgumentNullException(nameof(stream));

            _stream = stream;

            try
            {
                Running = true;
                _stream.BeginRead(_readBuffer, 0, _readBuffer.Length, new AsyncCallback(ReceiveCallback), null);
            }
            catch (Exception ex)
            {
                OnError(ex);
                Stop();
            }
        }

        public void Stop()
        {
            Running = false;
            OnCompleted();
        }

        public IDisposable Subscribe(IObserver<INetworkMessage> observer)
        {
            _observers.Add(observer);
            return new ListRemover<IObserver<INetworkMessage>>(_observers, observer);
        }

        private void OnError(Exception ex)
        {
            foreach (var observer in _observers.ToArray())
                if (_observers.Contains(observer))
                    observer.OnError(ex);
        }

        private void OnCompleted()
        {
            foreach (var observer in _observers.ToArray())
                if (_observers.Contains(observer))
                    observer.OnCompleted();
        }

        private void OnNext(INetworkMessage message)
        {
            foreach (var observer in _observers.ToArray())
                if (_observers.Contains(observer))
                    observer.OnNext(message);
        }
    }
}
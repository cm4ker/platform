using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using ZenPlatform.ServerClientShared.Logging;

namespace ZenPlatform.ServerClientShared.Network
{
    public class Channel : IChannel
    {
        private byte[] _readBuffer;
        private int _bufferSize = 1024 * 4; // 4Kb
        private Stream _stream;
        private IMessageHandler _handler;
        private readonly IMessagePackager _packager;
        private readonly ILogger _logger;

        public bool Running { get; private set; } = false;


        public event Action<Exception> OnError;

        public Channel(IMessagePackager packager, ILogger<Channel> logger)
        {
            _logger = logger;
            _readBuffer = new byte[_bufferSize];
            _packager = packager;
        }

        public void SetHandler(IMessageHandler handler)
        {
            _handler = handler;
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
                        _handler.Receive(message, this);
                    }
                }

                if (Running)
                {
                    _stream.BeginRead(_readBuffer, 0, _readBuffer.Length, new AsyncCallback(ReceiveCallback), null);
                }
            }
            catch (Exception ex)
            {
                Stop();
                OnError?.Invoke(ex);
            }
        }

        // todo send async method
        public void Send(object message)
        {
            if (!Running) throw new InvalidOperationException("Channel must be running.");
            if (message == null) throw new ArgumentNullException(nameof(message));

            _logger.Trace(() => string.Format("To: ''; Type: '{0}'; Message: {1}",
                message.GetType().Name, JsonConvert.SerializeObject(message)));

            _stream.Write(_packager.PackMessage(message));
        }

        public void Start(Stream stream, IMessageHandler handler)
        {
            _handler = handler ?? throw new ArgumentNullException(nameof(handler));

            _stream = stream ?? throw new ArgumentNullException(nameof(stream));

            Running = true;

            _stream.BeginRead(_readBuffer, 0, _readBuffer.Length, new AsyncCallback(ReceiveCallback), null);
        }

        public void Stop()
        {
            Running = false;
        }
    }
}
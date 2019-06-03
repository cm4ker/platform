using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

namespace ZenPlatform.ServerClientShared.Network
{
    public class DataStream : Stream, IMessageHandler
    {
        private IChannel _channel;
        private Guid _id;
        private MemoryStream _memoryStream;
        private AutoResetEvent _waitReceive;
        private readonly object _readLock;
        private bool _canRead = true;
        private bool _canWrite = true;
        private bool _end = false;

        public DataStream(Guid id, IChannel channel)
        {
            _channel = channel;
            _id = id;
            _memoryStream = new MemoryStream();
            _waitReceive = new AutoResetEvent(false);
            _readLock = new object();
            _channel.OnError += channel_OnError;
        }

        private void channel_OnError(Exception obj)
        {
            _end = true;
            _canWrite = false;
            _waitReceive.Set();
        }

        protected override void Dispose(bool disposing)
        {
            
            Flush();
            base.Dispose(disposing);
        }

        public override bool CanRead => _canRead;

        public override bool CanSeek => false;

        public override bool CanWrite => _canWrite;

        public override long Length => throw new NotImplementedException();

        public override long Position { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public override void Flush()
        {
            if (_canWrite)
            {
                _channel.Send(new EndInvokeStreamNetworkMessage(_id));
                _canWrite = false;
            }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            if (!_end)
                _waitReceive.WaitOne();
            //else return 0;
            int readbyte = 0;
            lock (_readLock)
            {
                readbyte = _memoryStream.Read(buffer, offset, count);
                if ((_memoryStream.Position < _memoryStream.Length) && !_end)
                    _waitReceive.Set();
            }
            return readbyte;

        }

        public void Receive(object message, IChannel channel)
        {
            lock (_readLock)
            {
                if (message is DataStreamNetworkMessage data)
                {
                    var pos = _memoryStream.Position;
                    _memoryStream.Seek(0, SeekOrigin.End);
                    _memoryStream.Write(data.Data);
                    _memoryStream.Position = pos;
                    _waitReceive.Set();
                }
                if (message is EndInvokeStreamNetworkMessage end)
                {
                    _end = true ;
                    _waitReceive.Set();
                }
                   

            }
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotImplementedException();
        }

        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            //todo packege split by 4Kb 
            if (_canWrite)
                _channel.Send(new DataStreamNetworkMessage(_id, buffer.AsSpan(offset, count).ToArray()));
        }
    }
}

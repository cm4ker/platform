using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ZenPlatform.Core.Network
{
    public class SimpleMessagePackager: IMessagePackager
    {
        private Stream _bufferStream;
        private ISerializer _serializer;
        private readonly object _streamLock = new object();
        private const int MAX_MESSAGE_SIZE = 8 * 1024;
        

        public SimpleMessagePackager(ISerializer serializer)
        {
            
            _serializer = serializer;
            _bufferStream = new MemoryStream();
        }

        private byte[] Combine(params byte[][] arrays)
        {
            byte[] rv = new byte[arrays.Sum(a => a.Length)];
            int offset = 0;
            foreach (byte[] array in arrays)
            {
                System.Buffer.BlockCopy(array, 0, rv, offset, array.Length);
                offset += array.Length;
            }
            return rv;
        }

        public byte[] PackMessage(object message)
        {
            var data = _serializer.ToBytes(message);

            var size = BitConverter.GetBytes(data.Length);

            return Combine(size, data);
        }

        public IEnumerable<object> UnpackMessages(byte[] byteString)
        {
            List<object> list = new List<object>();
            lock (_streamLock)
            {
                _bufferStream.Write(byteString);

                
                
                    while (ReadStream(list)) { };
            }
            return list;
        }

        private bool ReadStream(List<object> list)
        {
            _bufferStream.Position = 0;
            if (_bufferStream.Length <= 4)
            {
                _bufferStream.Position = _bufferStream.Length;
                return false;
            }

            var binaryReader = new BinaryReader(_bufferStream);

            var messageSize = binaryReader.ReadInt32();

            if (messageSize == 0 || messageSize < 0 )
            {
                _bufferStream = new MemoryStream();
                return false;
            }

            if (messageSize > MAX_MESSAGE_SIZE)
            {
                _bufferStream = new MemoryStream();
                //todo добавить запись в лог о таких ситуациях(размер данных больше максимума)
                return false;
            }

            if (messageSize + 4 > _bufferStream.Length)
            {
                _bufferStream.Position = _bufferStream.Length;
                return false;
            }

            var messageblob = binaryReader.ReadBytes(messageSize);
            try
            {
                list.Add(_serializer.FromBytes(messageblob));
                
            }
            catch (Exception ex)
            {
                _bufferStream = new MemoryStream();
                throw ex;
            }

            var leftcount = (int)(_bufferStream.Length - _bufferStream.Position);

            var leftblob = binaryReader.ReadBytes(leftcount);

            _bufferStream = new MemoryStream();
            _bufferStream.Write(leftblob);


            return _bufferStream.Length > 4;
        }
    }
}

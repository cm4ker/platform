using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace ZenPlatform.Core.RPC
{
    public class ClientBuilder
    {
        private ISerializer _serializer;
        private string _target;

        internal ClientBuilder()
        {

        }

        public static ClientBuilder CreateBuilder()
        {
            return new ClientBuilder();
        }

        public ClientBuilder SetSerialiser(ISerializer serializer)
        {
            _serializer = serializer;
            return this;
        }

        public ClientBuilder SetServer(string target)
        {
            _target = target;

            return this;
        }

        public Client Build()
        {
            Channel channel = new Channel(_target, ChannelCredentials.Insecure);

            if (_serializer == null)
                _serializer = new DeflateJSONSerializer();
            
            var client = new Client(channel, _serializer);

            return client;
        }
    }
}

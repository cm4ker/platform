using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Grpc.Core;
using ZenPlatform.Core.Environment;

namespace ZenPlatform.Core.RPC
{
    public class ServerBuilder
    {
        private struct Host
        {
            public Host(string host, int port)
            {
                _host = host;
                _port = port;
            }

            public string _host;
            public int _port;
        }

        private ISerializer _serializer;
        //private PlatformEnvironment _environment;
        private List<Host> _hostList = new List<Host>();
        

        private ServerBuilder()
        { }


        public static ServerBuilder CreateBuilder()
        {
            return new ServerBuilder();
        }

        public ServerBuilder AddHost(string host, int port)
        {
            
            _hostList.Add(new Host(host, port));
            return this;
        }
        /*
        public ServerBuilder SetEnvironment(PlatformEnvironment environment)
        {
            _environment = environment;
            return this;
        }
        */

        public ServerBuilder SetSerialiser(ISerializer serializer)
        {
            _serializer = serializer;
            return this;
        }

        public Server Build()
        {
            Grpc.Core.Server _gRPCserver = new Grpc.Core.Server();

            _hostList.ForEach(s => _gRPCserver.Ports.Add(new ServerPort(s._host, s._port, ServerCredentials.Insecure)));

            if (_serializer == null)
                _serializer = new DeflateJSONSerializer();

            var server = new Server(_gRPCserver, _serializer);

            return server;
        }
    }
}

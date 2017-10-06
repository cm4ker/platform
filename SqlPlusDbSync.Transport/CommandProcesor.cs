using System;
using SqlPlusDbSync.Data.Database;
using SqlPlusDbSync.Platform;

namespace SqlPlusDbSync.Transport
{
    public class CommandProcesor
    {
        private readonly Client _client;

        public CommandProcesor(Client client)
        {
            _client = client;
        }

        public bool IsConnected => _client.Connected;

        public void ProcessCommand(TransportCommand cmd)
        {
            if (cmd is GreetingCommand)
            {
                var greetingCmd = cmd as GreetingCommand;
                _client.RemoteId = greetingCmd.PointId;
                //GreetingResponce();
            }
            else if (cmd is GreetingResponceCommand)
            {
                var greetingCmd = cmd as GreetingResponceCommand;
                _client.RemoteId = greetingCmd.PointId;
            }
            else if (cmd is ObjectRequest)
            {
                cmd.Execute(_client);
            }
            else if (cmd is SaveObjectsCommand)
            {
                cmd.Execute(_client);
            }
            else if (cmd is GetObjectsCommand)
            {
                cmd.Execute(_client);
            }
            else if (cmd is ExitCommand)
            {
                _client.Close();
            }
        }

        public void StartProcess()
        {
            while (_client.Connected)
            {
                var data = _client.ReadData();
                var cmd = TransportHelper.Unpack<TransportCommand>(data);
                Console.WriteLine($"Incomming new command: {cmd.GetType()}");
                ProcessCommand(cmd);
            }
        }

        //public void Greeting()
        //{
        //    using (var context = new AsnaDatabaseContext())
        //    {
        //        var c = new Core(context);
        //        var gc = new GreetingCommand(context.PointId);
        //        _client.SendData(TransportHelper.Pack(gc));
        //    }
        //}

        //public void GreetingResponce()
        //{
        //    using (var context = new AsnaDatabaseContext())
        //    {
        //        var c = new Core(context);
        //        var gc = new GreetingResponceCommand(context.PointId);
        //        _client.SendData(TransportHelper.Pack(gc));
        //    }
        //}

        //public void ObjectRequest()
        //{
        //    using (var context = new AsnaDatabaseContext())
        //    {
        //        var or = new ObjectRequest();
        //        _client.SendData(TransportHelper.Pack(or));
        //    }
        //}

        //public void GetObjects()
        //{
        //    using (var context = new AsnaDatabaseContext())
        //    {
        //        var c = new Core(context);
        //        var goc = new GetObjectsCommand(context.PointId, context.GetSyncVersion(_client.RemoteId));
        //        _client.SendData(TransportHelper.Pack(goc));
        //    }
        //}

        //public void SendObjects()
        //{
        //    using (var context = new AsnaDatabaseContext())
        //    {
        //        var c = new Core(context);
        //        var objects = c.GetObjectPackageFromVersion(context.GetSyncVersion(_client.RemoteId), context.PointId);
        //        var soc = new SaveObjectsCommand(objects);
        //        _client.SendData(TransportHelper.Pack(soc));
        //    }
        //}

        public void ObjectRequestAccept()
        {

        }

        public void Exit()
        {
            var ec = new ExitCommand();
            _client.SendData(TransportHelper.Pack(ec));
        }
    }
}
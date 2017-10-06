using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using SqlPlusDbSync.Data.Database;
using SqlPlusDbSync.Platform;

namespace SqlPlusDbSync.Transport
{
    /// <summary>
    /// arg1 = pointid, arg2 = version
    /// </summary>
    public class GetObjectsCommand : TransportCommand<List<PlatformDynamicObject>>
    {
        /// <summary>
        /// arg0 = pointid(guid), arg1 = version(byte[])
        /// </summary>
        /// <param name="args"></param>
        public GetObjectsCommand(params object[] args) : base(args)
        {

        }

        public override string Name => nameof(GetObjectsCommand);


        [JsonIgnore]
        public List<Guid?> PointId => (List<Guid?>)Params["arg0"];
        [JsonIgnore]
        public byte[] Version => (byte[])Params["arg1"];

        public override List<PlatformDynamicObject> ExecuteCommand(Client client)
        {
            using (var context = new AsnaDatabaseContext())
            {
                var c = new Core(context);
                var objects = c.GetObjectPackageFromVersion(Version, PointId);

                var returnCmd = new SaveObjectsCommand(objects);
                client.SendData(TransportHelper.Pack(returnCmd));

                return objects;
            }
        }

        public override object Execute(Client client)
        {
            return ExecuteCommand(client);
        }
    }

    public class ObjectRequest : TransportCommand
    {
        public ObjectRequest(params object[] args) : base(args)
        {

        }

        public override string Name => "ObjectRequest";

        public override object Execute(Client client)
        {
            using (var context = new AsnaDatabaseContext())
            {
                var c = new Core(context);
                var goc = new GetObjectsCommand(context.PointId, context.GetSyncVersion(client.RemoteId));
                client.SendData(TransportHelper.Pack(goc));
                return null;
            }
        }
    }
}
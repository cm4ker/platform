using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using SqlPlusDbSync.Data.Database;
using SqlPlusDbSync.Platform;

namespace SqlPlusDbSync.Transport
{
    public class SaveObjectsCommand : TransportCommand<bool>
    {
        /// <summary>
        /// arg0 =  List of PlatformDynamicObject Objects
        /// </summary>
        public SaveObjectsCommand(params object[] args) : base(args)
        {
        }

        public override string Name
        {
            get { return "SaveCommand"; }
        }

        [JsonIgnore]
        public List<PlatformDynamicObject> Objects
        {
            get { return Params["arg0"] as List<PlatformDynamicObject>; }
        }

        public override object Execute(Client client)
        {
            return ExecuteCommand(client);
        }

        public override bool ExecuteCommand(Client client)
        {
            using (var context = new AsnaDatabaseContext())
            {
                var c = new Core(context);
                foreach (var dynamicObject in Objects)
                {
                    var sobject = c.SupportedObjects.Find(x => x.Name == dynamicObject[PlatformHelper.SObjectNameField].ToString());
                    if (sobject == null) throw new Exception("Rule not found");
                    try
                    {
                        c.SaveObject(dynamicObject, sobject);
                        c.SaveLastChangedVersion(dynamicObject, client.RemoteId);
                    }
                    catch (Exception)
                    {
                        return false;
                    }
                }

                return true;
            }
        }
    }
}
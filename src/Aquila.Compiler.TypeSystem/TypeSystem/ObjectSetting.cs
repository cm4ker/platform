using System;
using Aquila.Core.Contracts.TypeSystem;

namespace Aquila.Compiler.Aqua.TypeSystem
{
    public class ObjectSetting : IObjectSetting
    {
        public Guid ObjectId { get; set; }

        public uint SystemId { get; set; }

        public string DatabaseName { get; set; }
    }

    public class BackendObject : IBackendObject
    {
        public BackendObject(object cObject, Guid parentId)
        {
            PinnedObject = cObject;
            ParentId = parentId;
        }

        public Guid ParentId { get; }

        public object PinnedObject { get; }
    }
}
using System;
using ZenPlatform.Configuration.Contracts.TypeSystem;

namespace ZenPlatform.Configuration.Common.TypeSystem
{
    public class ObjectSetting : IObjectSetting
    {
        public Guid ObjectId { get; set; }

        public uint SystemId { get; set; }

        public string DatabaseName { get; set; }
    }
}
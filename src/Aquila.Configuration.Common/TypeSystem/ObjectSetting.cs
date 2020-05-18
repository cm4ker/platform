using System;
using Aquila.Core.Contracts.TypeSystem;

namespace Aquila.Configuration.Common.TypeSystem
{
    public class ObjectSetting : IObjectSetting
    {
        public Guid ObjectId { get; set; }

        public uint SystemId { get; set; }

        public string DatabaseName { get; set; }
    }
}
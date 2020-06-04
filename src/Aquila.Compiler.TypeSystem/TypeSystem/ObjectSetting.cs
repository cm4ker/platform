using System;
using Aquila.Core.Contracts.TypeSystem;

namespace Aquila.Compiler.Aqua.TypeSystem
{
    public class ObjectSetting 
    {
        public Guid ObjectId { get; set; }

        public uint SystemId { get; set; }

        public string DatabaseName { get; set; }
    }
}
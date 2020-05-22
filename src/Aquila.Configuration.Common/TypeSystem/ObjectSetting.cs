using System;
using Aquila.Compiler.Roslyn.RoslynBackend;
using Aquila.Core.Contracts.TypeSystem;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Aquila.Configuration.Common.TypeSystem
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
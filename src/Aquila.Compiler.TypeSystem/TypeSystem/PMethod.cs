using System;
using System.Collections.Generic;
using Aquila.Core.Contracts.TypeSystem;

namespace Aquila.Compiler.Aqua.TypeSystem
{
    public class PMethod : PInvokable
    {
        private Guid _typeId;

        internal PMethod(Guid id, Guid parentId, TypeManager tm) : base(id, parentId, tm)
        {
            Body = new CilBody();
        }

        internal PMethod(Guid parentId, TypeManager tm) : this(Guid.NewGuid(), parentId, tm)
        {
        }

        public IPType ReturnType => TypeManager.FindType(_typeId);

        public CilBody Body { get; }

        public void SetReturnType(Guid typeId)
        {
            _typeId = typeId;
        }

        public void SetReturnType(IPType type)
        {
            _typeId = type.Id;
        }
    }
}
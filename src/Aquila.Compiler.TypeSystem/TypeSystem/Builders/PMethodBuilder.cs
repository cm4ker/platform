using System;
using Aquila.Core.Contracts.TypeSystem;

namespace Aquila.Compiler.Aqua.TypeSystem.Builders
{
    public class PMethodBuilder : PMethod
    {
        private Guid _typeId;

        internal PMethodBuilder(Guid id, Guid parentId, TypeManager tm) : base(id, parentId, tm)
        {
            Body = new CilBody();
        }

        public override IPType ReturnType => TypeManager.FindType(_typeId);

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
using System;
using Aquila.Core.Contracts.TypeSystem;

namespace Aquila.Compiler.Aqua.TypeSystem.Builders
{
    public class PMethodBuilder : PMethod
    {
        private Guid _typeId;

        internal PMethodBuilder(Guid id, Guid parentId, TypeManager tm) : base(id, parentId, tm)
        {
            Body = new PCilBody();
        }

        public override PType ReturnType => TypeManager.FindType(_typeId);

        public PCilBody Body { get; }

        public void SetReturnType(Guid typeId)
        {
            _typeId = typeId;
        }

        public void SetReturnType(PType type)
        {
            _typeId = type.Id;
        }

        public void SetName(string name)
        {
            SetNameCore(name);
        }

        public PParameterBuilder DefineParameter()
        {
            var pb = new PParameterBuilder(TypeManager, Id);
            TypeManager.Register(pb);
            return pb;
        }
    }
}
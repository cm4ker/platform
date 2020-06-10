using System;
using Aquila.Core.Contracts.TypeSystem;

namespace Aquila.Compiler.Aqua.TypeSystem.Builders
{
    public sealed class PParameterBuilder : PParameter
    {
        private Guid _typeId;

        public PParameterBuilder(TypeManager tm, Guid invokableId) : base(tm, invokableId)
        {
        }

        public override PType Type => TypeManager.FindType(_typeId);

        public void SetType(PType type)
        {
            SetType(type.Id);
        }

        public void SetType(Guid typeId)
        {
            _typeId = typeId;
        }
    }
}
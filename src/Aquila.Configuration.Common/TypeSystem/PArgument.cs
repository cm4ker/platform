using System;
using Aquila.Core.Contracts.TypeSystem;
using dnlib.DotNet;

namespace Aquila.Configuration.Common.TypeSystem
{
    public class PArgument : IPArgument
    {
        private Guid _invokableId;
        private Guid _typeId;

        public PArgument(TypeManager tm, Guid invokableId)
        {
            TypeManager = tm;
            _invokableId = invokableId;
        }

        public string Name { get; set; }

        public IPType Type => TypeManager.FindType(_typeId);

        public IPInvokable Method => TypeManager.FindInvokable(_invokableId);

        public ITypeManager TypeManager { get; }

        public void SetType(IPType type)
        {
            SetType(type.Id);
        }

        public void SetType(Guid typeId)
        {
            _typeId = typeId;
        }
    }
}
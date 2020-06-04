using System;
using Aquila.Compiler.Contracts;
using Aquila.Core.Contracts.TypeSystem;

namespace Aquila.Compiler.Aqua.TypeSystem
{
    public class PParameter 
    {
        private Guid _invokableId;
        private Guid _typeId;

        public PParameter(TypeManager tm, Guid invokableId)
        {
            TypeManager = tm;
            _invokableId = invokableId;
        }

        public Guid Id { get; }

        public string Name { get; set; }

        public PType Type => TypeManager.FindType(_typeId);

        public PInvokable Method => TypeManager.FindInvokable(_invokableId);

        public TypeManager TypeManager { get; }

        public void SetType(PType type)
        {
            SetType(type.Id);
        }

        public void SetType(Guid typeId)
        {
            _typeId = typeId;
        }

        public IParameter BackendParameter { get; set; }
    }
}
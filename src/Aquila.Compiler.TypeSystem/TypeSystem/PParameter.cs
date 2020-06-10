using System;
using Aquila.Compiler.Contracts;
using Aquila.Core.Contracts.TypeSystem;

namespace Aquila.Compiler.Aqua.TypeSystem
{
    public class PParameter
    {
        private Guid _invokableId;


        public PParameter(TypeManager tm, Guid invokableId)
        {
            TypeManager = tm;
            _invokableId = invokableId;
        }

        public Guid Id { get; }

        public string Name { get; set; }

        public virtual PType Type => TypeManager.Unknown;

        public PInvokable Method => TypeManager.FindInvokable(_invokableId);

        internal Guid InvokableId => _invokableId;

        public TypeManager TypeManager { get; }


        public IParameter BackendParameter { get; set; }
    }
}
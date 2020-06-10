using System;
using System.Runtime.CompilerServices;
using Aquila.Core.Contracts.TypeSystem;

namespace Aquila.Compiler.Aqua.TypeSystem
{
    public class PMember
    {
        internal PMember(Guid id, Guid parentId, TypeManager tm)
        {
            TypeManager = tm;
            ParentId = parentId;
            Id = id;
        }

        public virtual bool IsProperty => false;

        public virtual bool IsMethod => false;

        public virtual bool IsConstructor => false;

        public Guid Id { get; }

        public Guid ParentId { get; }

        public PType DeclaringType => TypeManager.FindType(ParentId);

        public virtual string Name { get; private set; }

        public TypeManager TypeManager { get; }


        protected void SetNameCore(string name)
        {
            Name = name;
        }
    }
}
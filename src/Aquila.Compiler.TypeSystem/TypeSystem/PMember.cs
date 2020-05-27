using System;
using Aquila.Core.Contracts.TypeSystem;

namespace Aquila.Compiler.Aqua.TypeSystem
{
    public class PMember : IPMember
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

        public string Name { get; set; }

        public ITypeManager TypeManager { get; }
    }
}
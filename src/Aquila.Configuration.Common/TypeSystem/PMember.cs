using System;
using Aquila.Core.Contracts.TypeSystem;
using dnlib.DotNet;

namespace Aquila.Configuration.Common.TypeSystem
{
    public class PMember : IPMember
    {
        internal PMember(Guid parentId, TypeManager tm) : this(Guid.NewGuid(), parentId, tm)
        {
        }

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
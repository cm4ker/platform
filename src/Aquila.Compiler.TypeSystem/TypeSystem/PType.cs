using System;
using System.Collections.Generic;
using System.Linq;
using Aquila.Compiler.Contracts;
using Aquila.Core.Contracts.TypeSystem;

namespace Aquila.Compiler.Aqua.TypeSystem
{
    public abstract class PType
    {
        private readonly TypeManager _ts;

        internal PType(TypeManager ts)
        {
            _ts = ts;
        }

        public virtual Guid Id => _ts.Unknown.Id;

        public virtual Guid? ParentId { get; private set; }

        public virtual Guid? BaseId { get; } = null;

        public virtual Guid? GroupId { get; } = null;

        public virtual Guid? ComponentId { get; } = null;

        public virtual string Name { get; private set; }

        public virtual string Namespace { get; }

        public string FullName => $"{Namespace}.{Name}";

        public virtual bool IsDbAffect => false;

        public virtual ScopeAffects Scope => ScopeAffects.Unknown;

        public virtual bool IsPrimitive => false;

        public virtual bool IsAbstract => false;

        public virtual PrimitiveKind PrimitiveKind => PrimitiveKind.Unknown;

        public virtual bool IsValue => false;

        public virtual bool IsSizable => false;

        public virtual bool IsScalePrecision => false;

        public virtual bool IsTypeSpec => false;

        public virtual bool IsStatic => false;

        public virtual bool IsPublic => false;

        public virtual bool IsArray => false;

        public virtual bool IsGeneric => false;

        public virtual bool IsTypeSet => false;

        internal virtual IType BackendType { get; set; }

        /// <summary>
        /// This type is nested?
        /// </summary>
        public virtual bool IsNestedType => ParentId != null;

        public object Bag { get; set; }


        public virtual IEnumerable<PProperty> Properties => _ts.Properties.Where(x => x.ParentId == Id);

        public virtual IEnumerable<PMethod> Methods => _ts.Methods.Where(x => x.ParentId == Id);

        public virtual IEnumerable<PConstructor> Constructors => _ts.Constructors.Where(x => x.ParentId == Id);

        public virtual IEnumerable<PField> Fields => _ts.Fields.Where(x => x.ParentId == Id);

        public PTypeSpec GetSpec()
        {
            return _ts.DefineType(this);
        }

        protected void SetNameCore(string name)
        {
            Name = name;
        }

        protected void SetParentIdCore(Guid? parentId)
        {
            ParentId = parentId;
        }

        public TypeManager TypeManager => _ts;
    }
}
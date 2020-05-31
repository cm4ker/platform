using System;
using System.Collections.Generic;
using System.Linq;
using Aquila.Core.Contracts.TypeSystem;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Newtonsoft.Json.Bson;

namespace Aquila.Compiler.Aqua.TypeSystem
{
    public abstract class PType : IPType
    {
        private readonly TypeManager _ts;

        internal PType(TypeManager ts)
        {
            _ts = ts;
        }

        public virtual Guid Id => _ts.Unknown.Id;

        public virtual Guid? ParentId => null;

        public virtual Guid? BaseId { get; } = null;

        public virtual Guid? GroupId { get; } = null;

        public virtual Guid? ComponentId { get; } = null;

        public virtual string Name { get; set; }

        public virtual string Namespace { get; set; }

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

        public virtual bool IsArray => false;

        public virtual bool IsTypeSet => false;

        public virtual bool IsNestedType => false;

        public object Bag { get; set; }

        public virtual IEnumerable<IPMember> Members => GetMembers();

        public virtual IEnumerable<IPProperty> Properties => _ts.Properties.Where(x => x.ParentId == Id);

        public virtual IEnumerable<IPInvokable> Methods => _ts.Methods.Where(x => x.ParentId == Id);

        public virtual IEnumerable<ITable> Tables => _ts.Tables.Where(x => x.ParentId == Id);

        private IEnumerable<IPMember> GetMembers()
        {
            return _ts.Members.Where(x => x.ParentId == Id);
        }

        public IPTypeSpec GetSpec()
        {
            return _ts.Type(this);
        }

        public ITypeManager TypeManager => _ts;
    }
}
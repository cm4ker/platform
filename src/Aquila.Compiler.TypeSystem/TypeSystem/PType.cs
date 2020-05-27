using System;
using System.Collections.Generic;
using System.Linq;
using Aquila.Core.Contracts.TypeSystem;

namespace Aquila.Compiler.Aqua.TypeSystem
{
    public class PType : IPType
    {
        private readonly TypeManager _ts;

        internal PType(TypeManager ts)
        {
            _ts = ts;
        }


        public virtual Guid Id { get; }

        public virtual Guid? ParentId => null;

        public virtual Guid? BaseId { get; set; }

        public virtual Guid? GroupId { get; set; }

        public virtual Guid ComponentId { get; set; }

        public virtual string Name { get; set; }

        public virtual string Namespace { get; set; }

        public string FullName => $"{Namespace}.{Name}";

        public virtual bool IsUX { get; set; }

        public bool IsDbAffect { get; set; }

        public virtual bool IsPrimitive => false;

        public virtual bool IsAbstract => false;

        public virtual PrimitiveKind PrimitiveKind => PrimitiveKind.Unknown;

        public virtual bool IsValue { get; set; }

        public virtual bool IsSizable { get; set; }

        public virtual bool IsScalePrecision { get; set; }

        //TODO: Use scope in main project
        public bool IsAsmAvailable { get; set; }

        public bool IsQueryAvailable { get; set; }

        public virtual bool IsTypeSpec => false;

        public virtual bool IsArray => false;

        public virtual bool IsTypeSet => false;

        public virtual bool IsNestedType => false;

        internal bool IsRegistered { get; set; }

        public object Bag { get; set; }

        public IEnumerable<IPMember> Members => GetMembers();

        public IEnumerable<IPProperty> Properties => _ts.Properties.Where(x => x.ParentId == Id);

        public IEnumerable<IPInvokable> Methods => _ts.Methods.Where(x => x.ParentId == Id);

        public IEnumerable<ITable> Tables => _ts.Tables.Where(x => x.ParentId == Id);

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
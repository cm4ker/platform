using System;
using System.Collections.Generic;
using System.Linq;
using ZenPlatform.Configuration.Contracts.TypeSystem;

namespace ZenPlatform.Configuration.Common.TypeSystem
{
    public class PType : IPType
    {
        private readonly ITypeManager _ts;

        internal PType(ITypeManager ts)
        {
            _ts = ts;
        }

        public virtual Guid Id { get; set; }

        public virtual Guid? BaseId { get; set; }

        public virtual Guid? GroupId { get; set; }

        public virtual Guid ComponentId { get; set; }

        public virtual uint SystemId { get; set; }

        public virtual string Name { get; set; }

        public virtual bool IsLink { get; set; }

        public virtual bool IsObject { get; set; }

        public virtual bool IsManager { get; set; }

        public virtual bool IsDto { get; set; }
        
        public virtual bool IsUX { get; set; }

        public bool IsDbAffect { get; set; }

        public virtual bool IsPrimitive => false;

        public virtual bool IsAbstract => false;

        public virtual PrimitiveKind PrimitiveKind => PrimitiveKind.None;

        public virtual bool IsValue { get; set; }

        public virtual bool IsSizable { get; set; }

        public virtual bool IsScalePrecision { get; set; }

        //TODO: Use scope in main project
        public bool IsAsmAvaliable { get; set; }

        public bool IsQueryAvaliable { get; set; }

        public virtual bool IsTypeSpec => false;

        internal bool IsRegistrated { get; set; }

        public object Bag { get; set; }

        public IEnumerable<IPProperty> Properties => _ts.Properties.Where(x => x.ParentId == Id);
        public IEnumerable<ITable> Tables => _ts.Tables.Where(x => x.ParentId == Id);


        public IPTypeSpec GetSpec()
        {
            return _ts.Type(this);
        }

        public ITypeManager TypeManager => _ts;
    }
}
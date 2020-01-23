using System;
using System.Collections.Generic;
using System.Linq;

namespace ZenPlatform.Configuration.TypeSystem
{
    public class Type
    {
        private readonly TypeSystem _ts;

        internal Type(TypeSystem ts)
        {
            _ts = ts;
        }

        public virtual Guid Id { get; set; }

        public virtual Guid? ParentId { get; set; }

        public virtual uint SystemId { get; set; }

        public virtual string Name { get; set; }

        public virtual bool IsLink { get; set; }

        public virtual bool IsObject { get; set; }

        public virtual bool IsManager { get; set; }

        public virtual bool IsDto { get; set; }

        public virtual bool IsPrimitive { get; set; }

        public virtual bool IsValue { get; set; }

        public virtual bool IsSizable { get; set; }

        public virtual bool IsScalePrecision { get; set; }

        //TODO: Use scope in main project
        public bool IsCodeAccess { get; set; }

        public virtual bool IsTypeSpec => false;

        internal bool IsRegistrated { get; set; }

        public Metadata Metadata { get; set; }

        public object Bag { get; set; }

        public IEnumerable<Property> Properties => _ts.Properties.Where(x => x.ParentId == Id);
        public IEnumerable<Table> Tables => _ts.Tables.Where(x => x.ParentId == Id);


        public TypeSpec GetSpec()
        {
            return _ts.Type(this);
        }
    }
}
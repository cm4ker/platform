﻿using System;
using System.Collections.Generic;
using System.Linq;
using ZenPlatform.Configuration.Contracts.TypeSystem;

namespace ZenPlatform.Configuration.TypeSystem
{
    public class Type : IType
    {
        private readonly TypeManager _ts;

        internal Type(TypeManager ts)
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

        public IMetadata Metadata { get; set; }

        public object Bag { get; set; }

        public IEnumerable<IProperty> Properties => _ts.Properties.Where(x => x.ParentId == Id);
        public IEnumerable<ITable> Tables => _ts.Tables.Where(x => x.ParentId == Id);


        public ITypeSpec GetSpec()
        {
            return _ts.Type(this);
        }
    }
}
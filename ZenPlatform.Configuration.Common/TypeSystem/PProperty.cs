using System;
using System.Collections.Generic;
using System.Linq;
using ZenPlatform.Configuration.Contracts.TypeSystem;
using IPType = ZenPlatform.Configuration.Contracts.TypeSystem.IPType;

namespace ZenPlatform.Configuration.Common.TypeSystem
{
    public class PProperty : IPProperty
    {
        private readonly TypeManager _ts;

        internal PProperty(TypeManager ts)
        {
            _ts = ts;
        }

        public Guid Id { get; set; }

        public Guid ParentId { get; set; }

        public string Name { get; set; }

        public bool IsSelfLink { get; set; }

        public bool IsSystem { get; set; }

        public bool IsUnique { get; set; }

        public bool IsReadOnly { get; set; }

        public IEnumerable<IPType> Types => _ts.PropertyTypes
            .Where(x => x.PropertyId == Id && x.PropertyParentId == ParentId).Select(x => x.TypeId)
            .Join(_ts.Types, a => a, b => b.Id, (a, b) => b);

        public ITypeManager TypeManager => _ts;
    }
}
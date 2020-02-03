using System;
using System.Collections.Generic;
using System.Linq;
using ZenPlatform.Configuration.Contracts.TypeSystem;

namespace ZenPlatform.Configuration.TypeSystem
{
    public class Property : IProperty
    {
        private readonly TypeManager _ts;

        internal Property(TypeManager ts)
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

        public IEnumerable<IType> Types => _ts.PropertyTypes.Where(x => x.TypeId == Id).Select(x => x.TypeId)
            .Join(_ts.Types, a => a, b => b.Id, (a, b) => b);

        public IMDProperty Metadata { get; set; }
        public ITypeManager TypeManager => _ts;
    }
}
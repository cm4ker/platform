using System;
using System.Collections.Generic;
using System.Linq;
using ZenPlatform.Configuration.Common.TypeSystem;
using ZenPlatform.Configuration.Contracts.TypeSystem;

namespace ZenPlatform.Configuration.TypeSystem
{
    public class Table : ITable
    {
        private readonly TypeManager _ts;

        public Table(TypeManager ts)
        {
            _ts = ts;
        }

        public Guid Id { get; set; }

        public virtual uint SystemId { get; set; }
        
        public string Name { get; set; }

        public Guid ParentId { get; set; }

        public IEnumerable<IProperty> Properties => _ts.Properties.Where(x => x.ParentId == Id);
        public ITypeManager TypeManager { get; }
    }
}
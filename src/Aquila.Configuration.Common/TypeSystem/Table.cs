using System;
using System.Collections.Generic;
using System.Linq;
using Aquila.Configuration.Contracts.TypeSystem;

namespace Aquila.Configuration.Common.TypeSystem
{
    public class Table : ITable
    {
        private readonly TypeManager _ts;

        public Table(TypeManager ts)
        {
            _ts = ts;
        }

        public Guid Id { get; set; }
        public Guid GroupId { get; set; }

        public virtual uint SystemId { get; set; }
        
        public string Name { get; set; }

        public Guid ParentId { get; set; }

        public IEnumerable<IPProperty> Properties => _ts.Properties.Where(x => x.ParentId == Id);
        public ITypeManager TypeManager => _ts;
    }
}
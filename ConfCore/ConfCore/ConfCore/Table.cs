using System;
using System.Collections.Generic;
using System.Linq;

namespace ConfCore
{
    public class Table
    {
        private readonly TypeSystem _ts;

        public Table(TypeSystem ts)
        {
            _ts = ts;
        }

        public Guid Id { get; set; }

        public string Name { get; set; }

        public Guid ParentId { get; set; }

        public IEnumerable<Property> Properties => _ts.Properties.Where(x => x.ParentId == Id);
    }
}
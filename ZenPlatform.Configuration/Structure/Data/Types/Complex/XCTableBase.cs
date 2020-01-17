using System;
using System.Collections.Generic;
using ZenPlatform.Configuration.Contracts;

namespace ZenPlatform.Configuration.Structure.Data.Types.Complex
{
    public abstract class XCTableBase : IXCTable
    {
        public Guid Guid { get; set; }

        public uint Id { get; set; }

        public virtual IXCObjectType ParentType { get; }

        public virtual string Name { get; }

        public IEnumerable<IXCProperty> GetProperties()
        {
            throw new NotImplementedException();
        }
    }
}
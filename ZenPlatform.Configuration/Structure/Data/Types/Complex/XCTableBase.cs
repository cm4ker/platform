using System;
using System.Collections.Generic;
using ZenPlatform.Configuration.Contracts;

namespace ZenPlatform.Configuration.Structure.Data.Types.Complex
{
    public abstract class XCTableBase : IXCTable
    {
        public virtual Guid Guid { get; set; }

        public virtual uint Id { get; set; }

        public virtual IXCObjectType ParentType => throw new NotImplementedException();

        public virtual string Name { get; set; }

        /// <summary>
        /// Связанная таблица базы данных
        /// </summary>
        public virtual string RelTableName { get; set; }

        public virtual IEnumerable<IXCProperty> GetProperties()
        {
            throw new NotImplementedException();
        }
    }
}
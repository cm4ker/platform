using System;
using System.Collections.Generic;
using ZenPlatform.Configuration.Contracts;

namespace ZenPlatform.Configuration.Structure.Data.Types.Complex
{
    public class MDTable
    {
        public virtual Guid Guid { get; set; }

        public virtual MDType ParentType => throw new NotImplementedException();

        public virtual string Name { get; set; }

        /// <summary>
        /// Связанная таблица базы данных
        /// </summary>
        public virtual string RelTableName { get; set; }

        public virtual IEnumerable<MDProperty> GetProperties()
        {
            throw new NotImplementedException();
        }
    }
}
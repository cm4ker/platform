using System;
using System.Collections.Generic;
using System.Text;

namespace Aquila.Core.CacheService
{
    public class EmptyCacheService : ICacheService
    {
        public T Get<T>(int databaseId, int typeId, Guid entityId)
        {
            return default;
        }

        public object Get(Type type, int databaseId, int typeId, Guid entityId)
        {
            return null;
        }

        public void Set(int databaseId, int typeId, Guid entityId, object value)
        {
        }
    }
}
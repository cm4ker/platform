using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Runtime.Caching;

namespace Aquila.DataComponent.QueryBuilders
{
    public class ComponentCache : MemoryCache
    {
        public ComponentCache(string name, NameValueCollection config = null) : base(name, config)
        {
        }

        public ComponentCache(string name, NameValueCollection config, bool ignoreConfigSection) : base(name, config,
            ignoreConfigSection)
        {
        }
    }
}
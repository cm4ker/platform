using System;
using System.Collections.Generic;
using System.Text;

namespace ZenPlatform.QueryBuilder
{
    internal enum MachineContextType
    {
        none = 0,
        select,
        from,
        where,
        orderBy,
        groupBy,
        having,



    }

    internal class MachineContext
    {
        public MachineContextType Type { get; set; }

        
    }
}

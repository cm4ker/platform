using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Xml.Serialization;
using ZenPlatform.Configuration.Common;
using ZenPlatform.Configuration.Contracts;
using ZenPlatform.Configuration.TypeSystem;
using ZenPlatform.Shared.ParenChildCollection;

namespace ZenPlatform.Configuration.Structure.Data.Types.Complex
{
    public abstract class MDObject : Metadata
    {
        public virtual string RelTableName { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Text;
using ZenPlatform.Configuration;
using ZenPlatform.Configuration.Data;

namespace ZenPlatform.DataComponent.Configuration
{
    public class DefaultObjectRule : PObjectRule
    {
        public DefaultObjectRule(Guid objectId, PComponent component) : base(objectId, component)
        {
        }
    }
}

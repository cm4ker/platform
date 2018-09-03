using System;
using System.Collections.Generic;
using System.Text;
using ZenPlatform.Configuration.Contracts;
using ZenPlatform.Configuration.Structure.Data.Types.Complex;

namespace ZenPlatform.DataComponent.Configuration
{
    public class ConfigurationManagerBase : IXComponentManager
    {
        public virtual XCObjectTypeBase Create(XCObjectTypeBase baseType = null)
        {
            throw new NotImplementedException();
        }

        public virtual void Delete(XCObjectTypeBase type)
        {
            throw new NotImplementedException();
        }
    }
}
using System;
using System.Collections.Generic;
using System.Text;
using ZenPlatform.Configuration.Contracts;
using ZenPlatform.Configuration.Structure.Data.Types.Complex;

namespace ZenPlatform.DataComponent.Configuration
{
    public class ConfigurationManagerBase : IXComponentManager
    {
        public XCObjectTypeBase Create(XCObjectTypeBase parentType = null)
        {
            throw new NotImplementedException();
        }

        public void Delete(XCObjectTypeBase type)
        {
            throw new NotImplementedException();
        }
    }
}
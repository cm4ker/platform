using System;
using System.Collections.Generic;
using System.Text;
using ZenPlatform.Configuration.Data;
using ZenPlatform.Configuration.Data.Types.Complex;

namespace ZenPlatform.Configuration
{
    public interface IConfigurationManagerComponent
    {
        DataComponentObject ConfigurationUnloadHandler(PObjectType pobject);
        PObjectType ConfigurationComponentObjectLoadHandler(PComponent component, DataComponentObject componentObject);
        void ConfigurationObjectPropertyLoadHandler(PObjectType pObject, DataComponentObjectProperty objectProperty,
            List<PTypeBase> registeredTypes);
    }
}

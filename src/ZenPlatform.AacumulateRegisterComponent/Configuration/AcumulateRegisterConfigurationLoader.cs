using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using ZenPlatform.AcumulateRegisterComponent.Configuration.XmlConfiguration;
using ZenPlatform.Configuration.ConfigurationLoader.Contracts;
using ZenPlatform.Configuration.Data;
using ZenPlatform.Configuration.Exceptions;
using ZenPlatform.DataComponent.Configuration;
using ZenPlatform.DocumentComponent.Configuration;

namespace ZenPlatform.AcumulateRegisterComponent.Configuration
{
    public class AcumulateRegisterConfigurationLoader : ConfigurationLoaderBase
        <XmlConfAcumulateRegister, PAcumulateRegisterObjectType, PAcumulateRegisterObjectProperty>
    {
        protected override IComponentType CreateNewComponentType(PComponent component, XmlConfAcumulateRegister conf)
        {
            return new PAcumulateRegisterObjectType(conf.Name, conf.Guid, component);
        }

        protected override void RelsolveDependencies(PAcumulateRegisterObjectType obj, XmlConfAcumulateRegister conf, List<IComponentType> anotherDeps)
        {
            
        }
    }

}

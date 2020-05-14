using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using Aquila.AcumulateRegisterComponent.Configuration.XmlConfiguration;
using Aquila.Configuration.ConfigurationLoader.Contracts;
using Aquila.Configuration.Data;
using Aquila.Configuration.Exceptions;
using Aquila.DataComponent.Configuration;
using Aquila.DocumentComponent.Configuration;

namespace Aquila.AcumulateRegisterComponent.Configuration
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

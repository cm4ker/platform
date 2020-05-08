using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using Aquila.Configuration.ConfigurationLoader.Contracts;
using Aquila.Configuration.Data;
using Aquila.Configuration.Exceptions;
using Aquila.DataComponent.Configuration;
using Aquila.InformationRegisterComponent.Configuration.XmlConfiguration;

namespace Aquila.InformationRegisterComponent.Configuration
{
    public class InformationRegisterConfigurationLoader : ConfigurationLoaderBase<
        XmlConfInformationRegister, PInformationRegisterObjectType, PInformationRegisterObjectProperty>
    {
        protected override IComponentType CreateNewComponentType(PComponent component, XmlConfInformationRegister conf)
        {
            return new PInformationRegisterObjectType(conf.Name, conf.Guid, component);
        }
    }

}
